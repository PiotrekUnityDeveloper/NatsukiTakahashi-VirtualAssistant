using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [HideInInspector] public GameObject gameobjectRef;

    private DialogManager dialogManager;

    [HideInInspector] public int hours;
    [HideInInspector] public int minutes;
    [HideInInspector] public int seconds;

    [HideInInspector] public int hoursLeft;
    [HideInInspector] public int minutesLeft;
    [HideInInspector] public int secondsLeft;
    [HideInInspector] public int milliSecondsLeft;

    public TMP_Text timerName;
    public TMP_Text timerTime;

    public int totalTimeInSeconds = 0;

    public Stopwatch stopwatch;
    private bool isRunning = false;
    public TimerApp timerApp;
    public CMenu timerActions;

    private int originalDuration;

    [HideInInspector] public string characterPrompt = "Hey! it's time for [timerName]!";
    [HideInInspector] public string timerTitle = "NewTimer";

    private void Awake()
    {
        dialogManager = GameObject.FindFirstObjectByType<DialogManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameobjectRef = this.gameObject;
        //timerApp = this.GetComponentInParent<TimerApp>();
        //timerActions = GameObject.Find("TimerActions").GetComponent<CMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatch == null)
            return;

        float remainingTime = totalTimeInSeconds - (float)stopwatch.Elapsed.TotalSeconds;

        float remainingHours = Mathf.Floor(remainingTime / 3600);
        float remainingMinutes = Mathf.Floor((remainingTime % 3600) / 60);
        float remainingSeconds = Mathf.Floor(remainingTime % 60);
        float remainingMilliSeconds = 1000 - stopwatch.Elapsed.Milliseconds;

        hoursLeft = Mathf.FloorToInt(remainingHours);
        minutesLeft = Mathf.FloorToInt(remainingMinutes);
        secondsLeft = Mathf.FloorToInt(remainingSeconds);
        milliSecondsLeft = Mathf.FloorToInt(remainingMilliSeconds);

        UpdateTimer();

        if (remainingTime <= 0)
        {
            stopwatch.Stop();
            timerTime.text = "0s";
            TriggerAlarm();
            timerApp.RemoveTimer(this);
        }
    }

    public void UpdateTimer()
    {
        if(hoursLeft > 0)
        {
            timerTime.text = (hoursLeft < 10 ? "0" + hoursLeft.ToString() : hoursLeft.ToString()) + ":" +
                (minutesLeft < 10 ? "0" + minutesLeft.ToString() : minutesLeft.ToString()) + ":" +
                (secondsLeft < 10 ? "0" + secondsLeft.ToString() : secondsLeft.ToString());
        }
        else if(minutesLeft > 0)
        {
            timerTime.text = (minutesLeft.ToString().Length == 1 ? "0" + minutesLeft.ToString() : minutesLeft.ToString()) + ":" +
                (secondsLeft < 10 ? "0" + secondsLeft.ToString() : secondsLeft.ToString()) + " left"
                /*+ "<sup>" + milliSecondsLeft.ToString() + "</sup>"*/;
        }
        else
        {
            timerTime.text = (secondsLeft < 10 ? "0" + secondsLeft.ToString() : secondsLeft.ToString()) + "s left"
                /* + "<sup>" + milliSecondsLeft.ToString() + "ms</sup>"*/;
        }
    }

    public void InitTimer()
    {
        stopwatch = new Stopwatch();
        totalTimeInSeconds = (hours * 60 * 60) + (minutes * 60) + seconds;
        timerName.text = timerTitle;
        originalDuration = totalTimeInSeconds;
    }

    public void StartTimer()
    {
        isRunning = true;
        stopwatch.Start();
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            stopwatch.Stop();
        }
    }

    public void TriggerAlarm()
    {
        dialogManager.AddDialogToQueue(new DialogDefinition
        {
            message = characterPrompt.Replace("[timerName]", timerTitle),
            cooldown = 4f,
            showDelay = 0.5f,
            customVoiceKey = true,
            voiceKey = "reminder.wav",
        });

        dialogManager.StartQueue();
    }

    public void ToggleContext()
    {
        if (timerActions.isOpened)
        {
            if (timerActions.isActiveAndEnabled)
            {
                timerActions.HideMenu();
            }
            else
            {
                if (!timerActions.isActiveAndEnabled) timerActions.gameObject.SetActive(true);
                timerApp.contextMenuTarget = this;
                timerActions.ShowMenu();
                CharacterManager.MoveContextMenuToCursor(timerActions);
            }
        }
        else
        {
            if(!timerActions.isActiveAndEnabled) timerActions.gameObject.SetActive(true);
            timerApp.contextMenuTarget = this;
            timerActions.ShowMenu();
            CharacterManager.MoveContextMenuToCursor(timerActions);
        }
    }

    public GameObject focusedOverlay;

    public void Unfocus()
    {
        focusedOverlay.SetActive(false);
    }

    public void SetFocus()
    {
        //timerApp.SetFocusedTimer(this);
        focusedOverlay.SetActive(true);
    }

    public void ForceFocus()
    {
        timerApp.SetFocusedTimer(this);
    }

    public GameObject pauseMenu;
    private TimeSpan pausedTime;

    public bool isPaused = false;

    public void PauseTimer()
    {
        if(stopwatch == null)
            return;

        pausedTime = stopwatch.Elapsed;
        isPaused = true;
        stopwatch.Stop();
        pauseMenu.SetActive(true);
    }

    public void ResumeTimer()
    {
        if(stopwatch != null)
        {
            stopwatch.Elapsed.Add(pausedTime);
            isPaused = false;
            stopwatch.Start();
            pauseMenu.SetActive(false);
        }
    }

    public void AddTime(int seconds)
    {
        if(totalTimeInSeconds + seconds > 360999)
            return; // you cant bring it up more than 99 hours mate!

        totalTimeInSeconds += seconds;
    }

    public void ResetTimer()
    {
        if(stopwatch != null)
        {
            totalTimeInSeconds = originalDuration;
            stopwatch.Restart();
        }
    }

    public void RemoveTimer()
    {
        if(stopwatch != null)
        {
            stopwatch.Reset();
            stopwatch.Stop();
            Destroy(this.gameObject);
        }
    }
}
