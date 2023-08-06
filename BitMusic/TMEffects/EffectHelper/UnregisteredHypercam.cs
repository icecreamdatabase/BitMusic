using System;
using System.Runtime.InteropServices;

namespace BitMusic.TMEffects.EffectHelper;

public static class UnregisteredHypercam
{
    [DllImport("User32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("User32.dll")]
    private static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

    [DllImport("gdi32.dll")]
    static extern uint SetBkColor(IntPtr hdc, int crColor);

    [DllImport("gdi32.dll")]
    static extern uint SetTextColor(IntPtr hdc, int crColor);

    public static void ShowOverlay(string message)
    {
        IntPtr deviceContext = GetDC(IntPtr.Zero);
        SetBkColor(deviceContext, 0xFF_FF_FF);
        SetTextColor(deviceContext, 0x0);
        TextOut(deviceContext, 0, 0, message, message.Length);
        ReleaseDC(IntPtr.Zero, deviceContext);
    }

    public static void ClearOverlay() => ShowOverlay(string.Empty);
}
