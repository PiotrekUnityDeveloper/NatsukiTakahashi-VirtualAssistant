using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReminderApp : MonoBehaviour
{
    public List<Reminder> activeReminders = new List<Reminder>(); 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowReminderDialog()
    {

    }

    public TMP_InputField remName;
    public TMP_InputField remMsg;

    public void StructureReminder()
    {

    }

    public void AddReminder(Reminder reminder)
    {

    }

    public void UpdateReminders()
    {

    }
}

public class Reminder
{
    public string reminderName;
    public string reminderMessage;
    public DateTime reminderDate;
    public bool everyday;

    public bool mondayRepeat;
    public bool tuesdayRepeat;
    public bool wednesdayRepeat;
    public bool fridayRepeat;
    public bool saturdayRepeat;
    public bool sundayRepeat;

    public bool useAlarm;
    public bool alarmKey;
}

