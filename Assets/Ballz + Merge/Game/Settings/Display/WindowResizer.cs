using System;
using System.Runtime.InteropServices;

public static class WindowResizer
{
    public static bool IsResizable { get; private set; }

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_SIZEBOX = 0x00040000;
    private const int WS_MAXIMIZEBOX = 0x00010000;

#elif UNITY_STANDALONE_OSX
    private const int NSWindowStyleMaskResizable = 1 << 3; // 8

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_getClass")]
    private static extern IntPtr objc_getClass(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    private static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern long objc_msgSend_long(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_void_long(IntPtr receiver, IntPtr selector, long arg);
#endif

    public static void SetResizable(bool enable)
    {
#if UNITY_STANDALONE_WIN
        IntPtr handle = GetActiveWindow();
        int style = GetWindowLong(handle, GWL_STYLE);

        if (enable)
        {
            IsResizable = true;
            style |= WS_SIZEBOX | WS_MAXIMIZEBOX;
        }
        else
        {
            IsResizable = false;
            style &= ~(WS_SIZEBOX | WS_MAXIMIZEBOX);
        }

        SetWindowLong(handle, GWL_STYLE, style);

#elif UNITY_STANDALONE_OSX
        IntPtr nsAppClass = objc_getClass("NSApplication");
        IntPtr sharedAppSel = sel_registerName("sharedApplication");
        IntPtr nsApp = objc_msgSend(nsAppClass, sharedAppSel);

        IntPtr mainWindowSel = sel_registerName("mainWindow");
        IntPtr mainWindow = objc_msgSend(nsApp, mainWindowSel);

        IntPtr styleMaskSel = sel_registerName("styleMask");
        long styleMask = objc_msgSend_long(mainWindow, styleMaskSel);

        if (enable)
        {
            IsResizable = true;
            styleMask |= NSWindowStyleMaskResizable;
        }
        else
        {
            IsResizable = false;
            styleMask &= ~NSWindowStyleMaskResizable;
        }

        IntPtr setStyleMaskSel = sel_registerName("setStyleMask:");
        objc_msgSend_void_long(mainWindow, setStyleMaskSel, styleMask);
#endif
    }
}
