using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.LookAt;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Manages this Instance
    /// Used for character interactions
    /// </summary>


    // Reference for our character object!
    public GameObject character;
	private float initialWindowSize = 3f; //TODO: automatize or sum

    // Start is called before the first frame update
    void Start()
    {
        //character.GetComponent<CubismModel>().
        StartCoroutine(ScanProcesses());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrackingPoint();
    }

    // List of processes, found running on this computer, which match/contain the knownProcesses name.
    // This variable is fetched every 30 seconds.
    // We can use this to trigger certain character actions, like idk: "O!, i know this game! Im actually pretty good at it :D"
    public List<Process> avaiableProcesses = new List<Process>();

    // Add processes, which you want the character to recognize
    public static readonly List<string> knownProcesses = new List<string>()
    {
        "Spotify",
        "Minecraft",
        "Discord",
        "Chrome",
        "AudioStreamer",
        //"VisualStudio",adobeapps,bluestacks,nox(nobody use nox anyway),clipstudiopaint,krita,
        "Taskmgr",
        "SampleWindow",
        "firefox",
        "steamwebhelper",
        "EXPLORER",
        "vlc",
        "WinWord",
        "Excel",
        "POWERPNT"
    };
    
    // Win32 import START
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    // Win32 import END

    // Scan for processes on the computer, it they match the knownProcesses filter, then add them to the avaiableProcesses List
    private IEnumerator ScanProcesses()
    {
        // Get all running processes on this computer
        Process[] processlist = Process.GetProcesses();

        // List through all of them
        foreach (Process theprocess in processlist)
        {
            //print("Process: " + theprocess.ProcessName + " ID:" + theprocess.Id); // debug

            // Now, we iterate through a list of known processes, to know if 'theprocess' is a known process our character can recognize
            foreach(String s in knownProcesses)
            {
                //if 'theprocess' contains (or matches) any string from our list, then we add it!
                if (theprocess.ProcessName.Contains(s, StringComparison.OrdinalIgnoreCase)) // ignore process's name case
                {
                    avaiableProcesses.Add(theprocess); // Process matches the filter! Add it!!!
                    continue; // the process is found, so no need to searching further
                }
            }
        }

        // Wait X seconds before scanning again
        yield return new WaitForSeconds(30);
        StartCoroutine(ScanProcesses()); // after X seconds, scan again!
    }

    // Gameobject in which direction the character will look! :)
    public Transform trackingPoint;

    // Every frame, the 'trackingPoint' is moved to the exact cursor position, so the character looks at the cursor!
    // TODO: Manage 'trackingPoint' so its extensible, and we can tell it to look at something else :))
    public void UpdateTrackingPoint()
    {
        Vector3 cursorPosition = Input.mousePosition;
        cursorPosition.z = 10f;
        Vector3 worldPosition = SystemManager.instance.displayCamera.ScreenToWorldPoint(cursorPosition);
        trackingPoint.transform.position = worldPosition;
    }

    public void LaunchApp(GameObject g)
    {
        //g.SetActive(true);
        g.GetComponent<CanvasGroup>().alpha = 1;
        g.GetComponent<Image>().raycastTarget = true;
		g.GetComponent<BoxCollider2D>().isTrigger = false;
		g.transform.localScale = new Vector3(initialWindowSize, initialWindowSize, initialWindowSize);
    }

    public void ExitApp(GameObject g)
    {
        //g.SetActive(false);
        g.GetComponent<CanvasGroup>().alpha = 0;
        g.GetComponent<Image>().raycastTarget = false;
		g.GetComponent<BoxCollider2D>().isTrigger = true;
		g.transform.localScale = new Vector3(0, 0, 0);
    }

    public void ToggleApp(GameObject g)
    {
        //g.SetActive(!g.activeInHierarchy);
        if (g.GetComponent<CanvasGroup>().alpha > 0) // basically 1, was enabled
        {
            ExitApp(g);
        }
        else
        {
            LaunchApp(g);
        }
    }

    // EXTREMELY BASIC FOR NOW, IKKKK


}
