using System;
using System.Runtime.InteropServices;

public static class WindowResizerMac
{
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

    public static void SetResizable(bool enable)
    {
        IntPtr nsAppClass = objc_getClass("NSApplication");
        IntPtr sharedAppSel = sel_registerName("sharedApplication");
        IntPtr nsApp = objc_msgSend(nsAppClass, sharedAppSel);

        IntPtr mainWindowSel = sel_registerName("mainWindow");
        IntPtr mainWindow = objc_msgSend(nsApp, mainWindowSel);

        IntPtr styleMaskSel = sel_registerName("styleMask");
        long styleMask = objc_msgSend_long(mainWindow, styleMaskSel);

        if (enable)
            styleMask |= NSWindowStyleMaskResizable;
        else
            styleMask &= ~NSWindowStyleMaskResizable;

        IntPtr setStyleMaskSel = sel_registerName("setStyleMask:");
        objc_msgSend_void_long(mainWindow, setStyleMaskSel, styleMask);
    }
}
