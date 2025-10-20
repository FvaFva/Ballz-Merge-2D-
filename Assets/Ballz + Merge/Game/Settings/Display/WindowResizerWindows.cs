using System;
using System.Runtime.InteropServices;

public static class WindowResizerWindows
{
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
}
