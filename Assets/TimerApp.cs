using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TimerApp : MonoBehaviour
{
    public List<CountdownTimer> activeTimers = new List<CountdownTimer>();
    public CountdownTimer focusedTimer = null;

    public TMP_Text focusedTimerName;
    public TMP_Text focusedTimerTime;

    public GameObject timerPrefab;
    public CMenu timerActions;

    public CountdownTimer contextMenuTarget;
    public GameObject pauseIndicator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject firstTimeAdd;
    public GameObject clickToFocus;

    // Update is called once per frame
    void Update()
    {
        if(activeTimers.Count <= 0)
        {
            firstTimeAdd.SetActive(true);
        }
        else
        {
            firstTimeAdd.SetActive(false);
        }

        if(focusedTimer != null)
        {
            clickToFocus.SetActive(false);

            // update the main timer
            UpdateTimer();
        }
        else
        {
            pauseIndicator.SetActive(false);

            animatedImage.fillAmount = 1;

            if (activeTimers.Count > 0)
            {
                // they are just all unfocused
                clickToFocus.SetActive(true);
            }
            else
            {
                clickToFocus.SetActive(false);
            }
        }
    }

    public Image animatedImage;

    public void UpdateTimer()
    {
        //timer name/title
        focusedTimerName.text = focusedTimer.timerTitle;

        //timer current time
        if (focusedTimer.hoursLeft > 0)
        {
            focusedTimerTime.text = (focusedTimer.hoursLeft < 10 ? "0" + focusedTimer.hoursLeft.ToString() : focusedTimer.hoursLeft.ToString()) + ":" +
                (focusedTimer.minutesLeft < 10 ? "0" + focusedTimer.minutesLeft.ToString() : focusedTimer.minutesLeft.ToString()) + ":" +
                (focusedTimer.secondsLeft < 10 ? "0" + focusedTimer.secondsLeft.ToString() : focusedTimer.secondsLeft.ToString());
        }
        else if (focusedTimer.minutesLeft > 0)
        {
            focusedTimerTime.text = (focusedTimer.minutesLeft.ToString().Length == 1 ? "0" + focusedTimer.minutesLeft.ToString() : focusedTimer.minutesLeft.ToString()) + ":" +
                (focusedTimer.secondsLeft < 10 ? "0" + focusedTimer.secondsLeft.ToString() : focusedTimer.secondsLeft.ToString())
                + "<sup>" + (MillisecondsConversion(focusedTimer.milliSecondsLeft) < 10 ? "0" + MillisecondsConversion(focusedTimer.milliSecondsLeft).ToString() : MillisecondsConversion(focusedTimer.milliSecondsLeft).ToString()) + "</sup>";
        }
        else
        {
            focusedTimerTime.text = (focusedTimer.secondsLeft < 10 ? "0" + focusedTimer.secondsLeft.ToString() : focusedTimer.secondsLeft.ToString())
                + "<sup>" + (MillisecondsConversion(focusedTimer.milliSecondsLeft) < 10 ? "0" + MillisecondsConversion(focusedTimer.milliSecondsLeft).ToString() : MillisecondsConversion(focusedTimer.milliSecondsLeft).ToString()) + "ms</sup>";
        }

        if (focusedTimer.isPaused)
        {
            pauseIndicator.SetActive(true);
        }
        else
        {
            pauseIndicator.SetActive(false);
        }

        animatedImage.fillAmount = 1 - (float)CalculateProportion((double)focusedTimer.stopwatch.Elapsed.TotalSeconds, (double)focusedTimer.totalTimeInSeconds);

        // god damn
    }

    static double MillisecondsConversion(int milliseconds)
    {
        //double seconds = milliseconds / 1000.0;
        //return Math.Round(seconds * 60.0 / 1000.0, 2);
        return ((milliseconds / 100) * 6);
    }

    static double CalculateProportion(double amount, double totalAmount)
    {
        // dont divide by zero
        if (Math.Abs(totalAmount) < double.Epsilon)
            return 0;

        double proportion = amount / totalAmount;

        proportion = Math.Max(0, Math.Min(1, proportion));

        return proportion;
    }

    public GameObject timerPrompt;
    public TMP_InputField timerTitle;
    public TMP_InputField customPrompt;
    private int customHours;
    private int customMinutes;
    private int customSeconds;

    public TMP_Text hoursText;
    public TMP_Text minutesText;
    public TMP_Text secondsText;

    public GameObject timerHolder;

    public void AddToHours(int value)
    {
        customHours += value;

        if(customHours > 99)
        {
            customHours = 0;
        }

        if(customHours < 0)
        {
            customHours = 99;
        }

        hoursText.text = customHours < 10 ? "0" + customHours.ToString() : customHours.ToString();
    }

    public void AddToMinutes(int value)
    {
        customMinutes += value;

        if(customMinutes >= 60)
        {
            customMinutes = 0;
            AddToHours(1);
        }

        if(customMinutes < 0)
        {
            customMinutes = 59;
        }

        minutesText.text = customMinutes < 10 ? "0" + customMinutes.ToString() : customMinutes.ToString();
    }

    public void AddToSeconds(int value)
    {
        customSeconds += value;

        if (customSeconds >= 60)
        {
            customSeconds = 0;
            AddToMinutes(1);
        }

        if (customSeconds < 0)
        {
            customSeconds = 59;
        }

        secondsText.text = customSeconds < 10 ? "0" + customSeconds.ToString() : customSeconds.ToString();
    }

    public void AddNewTimerPrompt()
    {
        if (!timerPrompt.activeInHierarchy)
        {
            //reset data

            hoursText.text = "00";
            minutesText.text = "05";
            secondsText.text = "00";

            customHours = 0;
            customMinutes = 5;
            customSeconds = 0;

            customPrompt.text = "";
        }

        timerPrompt.SetActive(true);
    }

    public void HideTimerPrompt()
    {
        timerPrompt.SetActive(false);
    }

    private string[] randomTitles =
    {
        "NewTimer",
        "Unnamed",
        "Unnamed Timer",
        "Casual Countdown session",
        "Noname Timer :I",
    };

    public void AddNewTimerFromPrompt()
    {
        GameObject g = Instantiate(timerPrefab, timerHolder.transform);
        CountdownTimer ctd = g.GetComponent<CountdownTimer>();
        ctd.timerActions = this.timerActions;
        ctd.timerApp = this;
        ctd.hours = customHours;
        ctd.minutes = customMinutes;
        ctd.seconds = customSeconds;

        if (!string.IsNullOrWhiteSpace(timerTitle.text))
        {
            ctd.timerTitle = timerTitle.text;
        }
        else
        {
            ctd.timerTitle = randomTitles[UnityEngine.Random.Range(0, randomTitles.Length - 1)];
        }

        if (!string.IsNullOrWhiteSpace(customPrompt.text))
        {
            ctd.characterPrompt = customPrompt.text;
        }

        activeTimers.Add(ctd);
        SetFocusedTimer(ctd);

        ctd.InitTimer();
        ctd.StartTimer();
        HideTimerPrompt();
    }

    public void SetFocusedTimer(CountdownTimer timer)
    {
        UnfocusAll();
        focusedTimer = timer;
        timer.SetFocus();
    }

    public void UnfocusAll()
    {
        foreach(CountdownTimer timer in activeTimers)
        {
            timer.Unfocus();
        }
    }

    public void PauseTarget()
    {
        contextMenuTarget.PauseTimer();
    }

    public void AddTimeToTarget(int secondsToAdd)
    {
        contextMenuTarget.AddTime(secondsToAdd);
    }

    public void ResetTarget()
    {
        contextMenuTarget.ResetTimer();
    }

    public void CancelTarget()
    {
        RemoveTimer(contextMenuTarget);
        contextMenuTarget = null;
    }

    public void RemoveTimer(CountdownTimer timer)
    {
        if(focusedTimer == timer)
        {
            UnfocusAll();
            activeTimers.Remove(timer);
            timer.RemoveTimer();
            if(activeTimers.Count > 0) SetFocusedTimer(activeTimers[UnityEngine.Random.Range(0, activeTimers.Count - 1)]);
        }
        else
        {
            activeTimers.Remove(timer);
            timer.RemoveTimer();
        }

    }

    public void HideContextMenu()
    {
        timerActions.HideMenu();
    }

    public GameObject timerRemovalDialog;

    public void ShowClearingDialog(bool show)
    {
        timerRemovalDialog.SetActive(show);
    }

    public void ClearAllTimers()
    {
        UnfocusAll();

        foreach(CountdownTimer timer in activeTimers)
        {
            timer.RemoveTimer();
        }

        activeTimers.Clear();

        focusedTimerTime.text = "00:00";
        focusedTimerName.text = "";
    }
}
