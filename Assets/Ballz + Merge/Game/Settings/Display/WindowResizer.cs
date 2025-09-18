using System;
using System.Runtime.InteropServices;

public static class WindowResizer
{
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_SIZEBOX = 0x00040000;
    private const int WS_MAXIMIZEBOX = 0x00010000;

    public static void SetResizable(bool enable)
    {
        IntPtr handle = GetActiveWindow();
        int style = GetWindowLong(handle, GWL_STYLE);

        if (enable)
            style |= WS_SIZEBOX | WS_MAXIMIZEBOX;
        else
            style &= ~(WS_SIZEBOX | WS_MAXIMIZEBOX);

        SetWindowLong(handle, GWL_STYLE, style);
    }
#endif

#if UNITY_STANDALONE_OSX
    private const int NSWindowStyleMaskResizable = 1 << 3; // 8

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr NSApplicationSharedApplication();

    [DllImport("/usr/lib/libobjc.A.dylib")]
    private static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, int arg);

    public static void SetResizable(bool enable)
    {
        IntPtr nsApp = NSApplicationSharedApplication();

        IntPtr mainWindowSel = sel_registerName("mainWindow");
        IntPtr mainWindow = objc_msgSend(nsApp, mainWindowSel);

        IntPtr styleMaskSel = sel_registerName("styleMask");
        int styleMask = (int)objc_msgSend(mainWindow, styleMaskSel);

        if (enable)
            styleMask |= NSWindowStyleMaskResizable;
        else
            styleMask &= ~NSWindowStyleMaskResizable;

        IntPtr setStyleMaskSel = sel_registerName("setStyleMask:");
        objc_msgSend(mainWindow, setStyleMaskSel, styleMask);
    }
#endif
}
