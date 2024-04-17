using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReminderTimer : MonoBehaviour
{
    public DialogManager dialogManager;
    [HideInInspector] public ReminderApp app;

    public string reminderName;
    public string reminderMessage;
    public DateTime reminderDate;
    public bool useTime;
    //public bool everyday;

    public bool mondayRepeat;
    public bool tuesdayRepeat;
    public bool wednesdayRepeat;
    public bool thursdayRepeat;
    public bool fridayRepeat;
    public bool saturdayRepeat;
    public bool sundayRepeat;

    public bool useAlarm;
    public string alarmKey;

    private TMP_Text reminderNameTxt;
    private TMP_Text reminderDeadlineTxt;
    private GameObject disabledPanel;

    public CMenu reminderOpt;

    public void InitReminder()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTimer()
    {
        DateTime currentTime = DateTime.Now;

        if (useTime)
        {
            if(currentTime.Year == reminderDate.Year &&
                currentTime.Month == reminderDate.Month &&
                currentTime.Day == reminderDate.Day &&
                currentTime.Hour == reminderDate.Hour &&
                currentTime.Minute == reminderDate.Minute)
            {
                //trigger the reminder
                TriggerReminder();
            }
        }
        else
        {
            if (currentTime.Year == reminderDate.Year &&
                currentTime.Month == reminderDate.Month &&
                currentTime.Day == reminderDate.Day) // do not use the time
            {
                //trigger the reminder
                TriggerReminder();
            }
        }
    }

    public void TriggerReminder()
    {
        if (useAlarm) // custom key
        {
            dialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = reminderMessage.Replace("[reminderName]", reminderName),
                cooldown = 4f,
                showDelay = 0.5f,
                customVoiceKey = true,
                voiceKey = alarmKey,
            });
        }
        else
        {
            dialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = reminderMessage.Replace("[reminderName]", reminderName),
                cooldown = 4f,
                showDelay = 0.5f,
                customVoiceKey = true,
                voiceKey = "reminder.wav",
            });
        }

        dialogManager.StartQueue();
    }

    public void RemoveReminder()
    {
        Destroy(this.gameObject);
    }

    public void ToggleContext()
    {
        app.focusedReminder = this;

        //show the cm

        if (app.reminderActions.isOpened)
        {
            if (app.reminderActions.isActiveAndEnabled)
            {
                app.reminderActions.HideMenu();
            }
            else
            {
                if (!app.reminderActions.isActiveAndEnabled) app.reminderActions.gameObject.SetActive(true);
                app.reminderActions.ShowMenu();
                CharacterManager.MoveContextMenuToCursor(app.reminderActions);
            }
        }
        else
        {
            if (!app.reminderActions.isActiveAndEnabled) app.reminderActions.gameObject.SetActive(true);
            app.reminderActions.ShowMenu();
            CharacterManager.MoveContextMenuToCursor(app.reminderActions);
        }
    }

    //public GameObject disabledPanel;
    public bool isDisabled;

    public void DisableRem()
    {
        disabledPanel.SetActive(true);
        isDisabled = true;
    }

    public void EnableRem()
    {
        disabledPanel.SetActive(false);
        isDisabled = false;
    }


}
