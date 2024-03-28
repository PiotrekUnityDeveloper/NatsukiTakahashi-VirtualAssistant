using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Diagnostics;
using System.Drawing;

public class SystemManager : MonoBehaviour
{
    // Win32 import START
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    // Win32 import END

    // Win32 import START
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    // Win32 import END

    // Win32 Managed Struct for window borders (do not reorder!!)
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // Win32 import START
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    // Win32 import END

    // Win32 import START
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    // Win32 import END

    // System import START
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();
    // System import END

    // Win32 import START
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    // Win32 import END

    // Win32 import START
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, uint uFlags);
    // Win32 import END

    // Win32 import START
    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    // Win32 import END

    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    const uint LWA_COLORKEY = 0x00000001;

    private IntPtr hWnd;

    // Save resoluation state
    private Resolution previousResolution;

    public List<string> allowedProcesses = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        allowedProcesses = GameManager.knownProcesses;

        //InitializeBounds(); TODO: fix

        Setup();

        Application.runInBackground = true;

        #if !UNITY_EDITOR
        IntPtr hWnd = GetActiveWindow();
        //hWnd = GetCurrentProcess();
        // a value of -1 on any of the window margins, makes the backgrouond of the game's window transparent
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

        #endif
    }

    private void SetClickThrough(bool clickthrough)
    {
        if (clickthrough)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        SetClickThrough(Physics2D.OverlapPoint(worldPosition) == null);
        ResoultionUpdate();
    }

    public void Setup()
    {
        SetupScreen(); // setup the screen properties
    }

    public void SetupScreen()
    {
        // Get the native resolution of the display
        Resolution nativeResolution = Screen.currentResolution;
        // Set game's fullscreen resolution to the native display resolution
        Screen.SetResolution(nativeResolution.width, nativeResolution.height, true);
        Screen.fullScreen = true;
        previousResolution = Screen.currentResolution;
    }

    public void ResoultionUpdate()
    {
        //force the game's window to stay in fullscreen!
        if (!Screen.currentResolution.Equals(previousResolution))
        {
            Screen.fullScreen = true;
        }
    }

    /// BOUNDS MANAGER
    #region win32references
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    // Callback Declaration
    public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);
    [DllImport("user32.dll")]
    private static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentProcessId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, uint dwRop);

    [DllImport("gdi32.dll")]
    private static extern IntPtr DeleteDC(IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr DeleteObject(IntPtr hObject);

    #endregion win32references

    public GameObject windowPrefab;
    public Camera mainCamera;

    public void InitializeBounds()
    {
        uint currentProcessId = GetCurrentProcessId();

        foreach (Process process in Process.GetProcesses())
        {
            if (process.Id == currentProcessId)
                continue;

            bool isknown = false;

            foreach(string s in allowedProcesses)
            {
                if(process.ProcessName.Contains(s, StringComparison.OrdinalIgnoreCase))
                {
                    isknown = true;
                    allowedProcesses.Remove(s);
                    break;
                }
            }

            if(!isknown)
                continue;

            RECT rt = new RECT();
            bool locationLookupSucceeded = GetWindowRect(process.MainWindowHandle, out rt);

            if (locationLookupSucceeded)
            {
                // Calculate the width and height of the window
                int width = rt.Right - rt.Left;
                int height = rt.Bottom - rt.Top;

                // Calculate the position of the window's center
                int centerX = (rt.Left + rt.Right) / 2;
                int centerY = (rt.Top + rt.Bottom) / 2;

                if(width <= 0 || height <= 0)
                {
                    continue;
                }

                // Invert the Y-coordinate to match Unity's coordinate system
                int invertedTop = Screen.height - rt.Top;
                int invertedBottom = Screen.height - rt.Bottom;

                // Calculate the center of the window in screen coordinates
                Vector3 windowCenter = new Vector3((rt.Left + rt.Right) / 2f, (invertedTop + invertedBottom) / 2f, 0f);

                // Convert the screen coordinates to world coordinates based on the camera's viewport
                Vector3 worldCenter = mainCamera.ScreenToWorldPoint(windowCenter);

                // Calculate the width and height of the window in world coordinates
                Vector3 worldWidth = mainCamera.ScreenToWorldPoint(new Vector3(rt.Right, invertedTop, 0f)) - mainCamera.ScreenToWorldPoint(new Vector3(rt.Left, invertedTop, 0f));
                Vector3 worldHeight = mainCamera.ScreenToWorldPoint(new Vector3(rt.Left, invertedBottom, 0f)) - mainCamera.ScreenToWorldPoint(new Vector3(rt.Left, invertedTop, 0f));

                // Instantiate the square GameObject
                GameObject square = Instantiate(windowPrefab, worldCenter, Quaternion.identity);

                // Resize the square to match the window size
                square.transform.localScale = new Vector3(worldWidth.magnitude, worldHeight.magnitude, 1f);
                square.name = process.ProcessName;

                if (!IsWindowVisible(process.MainWindowHandle))
                {
                    square.SetActive(false);
                }
            }
        }
    }

    public bool IsProcessActive(Process process)
    {
        // Get the handle of the currently focused window
        IntPtr foregroundWindowHandle = GetForegroundWindow();

        // Compare the handle of the currently focused window with the process's main window handle
        return foregroundWindowHandle == process.MainWindowHandle;
    }

    public void UpdateBounds()
    {

    }

    //util

    public Sprite ConvertToSprite(Texture2D texture)
    {
        // Create a new Sprite using the provided texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }

    public Texture2D CaptureWindowScreenshot(Process process)
    {
        // Get the main window handle of the process
        IntPtr mainWindowHandle = process.MainWindowHandle;

        // Get the device context (DC) of the window
        IntPtr windowDC = GetWindowDC(mainWindowHandle);

        // Get the dimensions of the window
        RECT rect;
        GetWindowRect(mainWindowHandle, out rect);
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        // Create a compatible DC and bitmap
        IntPtr compatibleDC = CreateCompatibleDC(windowDC);
        IntPtr compatibleBitmap = CreateCompatibleBitmap(windowDC, width, height);

        // Select the bitmap into the compatible DC
        IntPtr oldBitmap = SelectObject(compatibleDC, compatibleBitmap);

        // Perform the bit-block transfer from the window DC to the compatible DC
        BitBlt(compatibleDC, 0, 0, width, height, windowDC, 0, 0, 0x00CC0020); // SRCCOPY

        // Create a new texture to hold the screenshot
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read the bitmap data from the compatible DC
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        // Clean up resources
        SelectObject(compatibleDC, oldBitmap);
        DeleteObject(compatibleBitmap);
        DeleteDC(compatibleDC);
        ReleaseDC(mainWindowHandle, windowDC);

        // Apply changes and return the screenshot texture
        texture.Apply();
        return texture;
    }

}
