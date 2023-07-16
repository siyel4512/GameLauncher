using System;
using System.IO;
using UnityEngine;

public class WindowSizeInitializer : MonoBehaviour
{
    public int initialScreenWidth = 800;
    public int initialScreenHeight = 600;

    private void Awake()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        //SetWindowSize(initialScreenWidth, initialScreenHeight);
        //SetWindowSize();
#endif
    }

    //    void Start()
    //    {
    //#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    //        SetWindowSize(initialScreenWidth, initialScreenHeight);
    //#endif
    //    }

    //    private void OnApplicationQuit()
    //    {
    //#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    //        SetWindowSize(initialScreenWidth, initialScreenHeight);
    //#endif
    //    }


    //    public void SetWindowSize(int width, int height)
    //    {
    //        int screenWidth = Screen.currentResolution.width;
    //        int screenHeight = Screen.currentResolution.height;
    //        int windowX = (screenWidth - width) / 2;
    //        int windowY = (screenHeight - height) / 2;

    //#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    //        var windowHandle = GetActiveWindow();
    //        //SetWindowPos(windowHandle, System.IntPtr.Zero + Screen.width, 0 + Screen.height, 0, width, height, SWP_SHOWWINDOW);
    //        SetWindowPos(windowHandle, IntPtr.Zero, windowX, windowY, width, height, SWP_SHOWWINDOW);
    //#endif
    //    }

    public void SetWindowSize()
    {
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;
        int windowX = (screenWidth - initialScreenWidth) / 2;
        int windowY = (screenHeight - initialScreenHeight) / 2;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        var windowHandle = GetActiveWindow();
        //SetWindowPos(windowHandle, System.IntPtr.Zero + Screen.width, 0 + Screen.height, 0, width, height, SWP_SHOWWINDOW);
        SetWindowPos(windowHandle, IntPtr.Zero, windowX, windowY, initialScreenWidth, initialScreenHeight, SWP_SHOWWINDOW);
#endif
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetWindowPos(System.IntPtr hwnd, System.IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    private const uint SWP_SHOWWINDOW = 0x0040;
#endif
}