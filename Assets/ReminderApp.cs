using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderApp : MonoBehaviour
{
    public List<ReminderTimer> activeReminders = new List<ReminderTimer>();
    //public static DialogManager dialogManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateReminders());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TMP_InputField remName;
    public TMP_InputField remMsg;

    public TMP_Dropdown day;
    public TMP_Dropdown month;
    public TMP_Dropdown year;

    public TMP_Dropdown hour;
    public TMP_Dropdown minute;

    public Toggle useTime;

    //repeat
    public Toggle monday;
    public Toggle tuesday;
    public Toggle wednesday;
    public Toggle thursday;
    public Toggle friday;
    public Toggle saturday;
    public Toggle sunday;

    public Toggle useAlarm;
    public TMP_InputField alarmKey;

    public GameObject reminderPrefab;
    public Transform reminderContainer;

    public ReminderTimer focusedReminder;

    public CMenu reminderActions;

    public void AddReminder()
    {
        ReminderTimer newReminder;

        GameObject g = Instantiate(reminderPrefab, reminderContainer);
        newReminder = g.GetComponent<ReminderTimer>();

        // structure the date
        // beware terrible coding practice

        string dateString = day.value.ToString() + " " + month.value.ToString() + " " + year.value.ToString().Replace("-", string.Empty);
        
        // DATETIME
        DateTime dateTime = DateTime.ParseExact(dateString, "dd MMMM yyyy", CultureInfo.InvariantCulture);
        DateTime reminderDateTime;

        if (useTime)
        {
            reminderDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, Int32.Parse(hour.value.ToString()), Int32.Parse(minute.value.ToString()), 0);
        }
        else
        {
            reminderDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        newReminder.reminderDate = reminderDateTime;

        string reminderName = remName.text;
        string reminderMsg = remMsg.text;

        newReminder.reminderName = reminderName;
        newReminder.reminderMessage = reminderMsg;

        newReminder.mondayRepeat = monday.isOn;
        newReminder.tuesdayRepeat = tuesday.isOn;
        newReminder.wednesdayRepeat = wednesday.isOn;
        newReminder.thursdayRepeat = thursday.isOn;
        newReminder.fridayRepeat = friday.isOn;
        newReminder.saturdayRepeat = saturday.isOn;
        newReminder.sundayRepeat = sunday.isOn;

        newReminder.dialogManager = DialogManager.Instance;
        newReminder.app = this;

        newReminder.useAlarm = useAlarm.isOn;
        newReminder.alarmKey = alarmKey.text;

        newReminder.useTime = useTime;

        activeReminders.Add(newReminder);
    }

    public IEnumerator UpdateReminders()
    {

        foreach(ReminderTimer remtimer in activeReminders)
        {
            if (!remtimer.isDisabled)
            {
                remtimer.UpdateTimer();
            }
        }

        yield return new WaitForSeconds(10f);

        StartCoroutine(UpdateReminders());
    }

    public GameObject reminderDialog;

    public void ShowReminderDialog()
    {
        reminderDialog.SetActive(true);
    }

    public void HideReminderDialog()
    {
        reminderDialog.SetActive(false);
    }

    // too many settings for one page T-T
    public GameObject page1;
    public GameObject page2;

    public void TogglePages()
    {
        if (page1.activeInHierarchy)
        {
            page1.SetActive(false);
            page2.SetActive(true);
        }
        else
        {
            page1.SetActive(true);
            page2.SetActive(false);
        }
    }

    public void ResetPage()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void RemoveReminder(ReminderTimer rt)
    {
        rt.RemoveReminder();
    }

    public void RemoveFocused()
    {
        focusedReminder.RemoveReminder();
    }

    public void DisableFocused()
    {
        focusedReminder.DisableRem();
    }

    public void HideContext()
    {
        reminderActions.HideMenu();
    }
}

