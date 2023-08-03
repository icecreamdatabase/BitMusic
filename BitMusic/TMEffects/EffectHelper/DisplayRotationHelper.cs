using System;
using System.Runtime.InteropServices;

namespace BitMusic.TMEffects.EffectHelper;

public class DisplayRotationHelper
{
    public enum Orientations
    {
        DegreesCw0 = 0,
        DegreesCw90 = 3,
        DegreesCw180 = 2,
        DegreesCw270 = 1
    }

    public static bool Rotate(uint displayNumber, Orientations orientation)
    {
        if (displayNumber == 0)
            throw new ArgumentOutOfRangeException("displayNumber", displayNumber, "First display is 1.");

        bool result = false;

        DisplayDevice d = new DisplayDevice();
        d.cb = Marshal.SizeOf(d);

        Devmode dm = new Devmode();

        if (!NativeMethods.EnumDisplayDevices(null, displayNumber - 1, ref d, 0))
            throw new ArgumentOutOfRangeException("displayNumber", displayNumber,
                "Number is greater than connected displays.");

        if (0 != NativeMethods.EnumDisplaySettings(
                d.DeviceName, NativeMethods.EnumCurrentSettings, ref dm))
        {
            if ((dm.dmDisplayOrientation + (int)orientation) % 2 == 1) // Need to swap height and width?
            {
                (dm.dmPelsHeight, dm.dmPelsWidth) = (dm.dmPelsWidth, dm.dmPelsHeight);
            }

            switch (orientation)
            {
                case Orientations.DegreesCw90:
                    dm.dmDisplayOrientation = NativeMethods.Dmdo270;
                    break;
                case Orientations.DegreesCw180:
                    dm.dmDisplayOrientation = NativeMethods.Dmdo180;
                    break;
                case Orientations.DegreesCw270:
                    dm.dmDisplayOrientation = NativeMethods.Dmdo90;
                    break;
                case Orientations.DegreesCw0:
                    dm.dmDisplayOrientation = NativeMethods.DmdoDefault;
                    break;
                default:
                    break;
            }

            DispChange ret = NativeMethods.ChangeDisplaySettingsEx(
                d.DeviceName, ref dm, IntPtr.Zero,
                DisplaySettingsFlags.CdsUpdateregistry, IntPtr.Zero);

            result = ret == 0;
        }

        return result;
    }

    public static void ResetAllRotations()
    {
        try
        {
            uint i = 0;
            while (++i <= 64)
            {
                Rotate(i, Orientations.DegreesCw0);
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // Everything is fine, just reached the last display
        }
    }
}

internal class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern DispChange ChangeDisplaySettingsEx(
        string lpszDeviceName, ref Devmode lpDevMode, IntPtr hwnd,
        DisplaySettingsFlags dwflags, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern bool EnumDisplayDevices(
        string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice,
        uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    internal static extern int EnumDisplaySettings(
        string lpszDeviceName, int iModeNum, ref Devmode lpDevMode);

    public const int DmdoDefault = 0;
    public const int Dmdo90 = 1;
    public const int Dmdo180 = 2;
    public const int Dmdo270 = 3;

    public const int EnumCurrentSettings = -1;
}

// See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183565(v=vs.85).aspx
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
internal struct Devmode
{
    public const int Cchdevicename = 32;
    public const int Cchformname = 32;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchdevicename)]
    [FieldOffset(0)]
    public string dmDeviceName;

    [FieldOffset(32)]
    public short dmSpecVersion;

    [FieldOffset(34)]
    public short dmDriverVersion;

    [FieldOffset(36)]
    public short dmSize;

    [FieldOffset(38)]
    public short dmDriverExtra;

    [FieldOffset(40)]
    public Dm dmFields;

    [FieldOffset(44)]
    short dmOrientation;

    [FieldOffset(46)]
    short dmPaperSize;

    [FieldOffset(48)]
    short dmPaperLength;

    [FieldOffset(50)]
    short dmPaperWidth;

    [FieldOffset(52)]
    short dmScale;

    [FieldOffset(54)]
    short dmCopies;

    [FieldOffset(56)]
    short dmDefaultSource;

    [FieldOffset(58)]
    short dmPrintQuality;

    [FieldOffset(44)]
    public Pointl dmPosition;

    [FieldOffset(52)]
    public int dmDisplayOrientation;

    [FieldOffset(56)]
    public int dmDisplayFixedOutput;

    [FieldOffset(60)]
    public short dmColor;

    [FieldOffset(62)]
    public short dmDuplex;

    [FieldOffset(64)]
    public short dmYResolution;

    [FieldOffset(66)]
    public short dmTTOption;

    [FieldOffset(68)]
    public short dmCollate;

    [FieldOffset(72)]
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchformname)]
    public string dmFormName;

    [FieldOffset(102)]
    public short dmLogPixels;

    [FieldOffset(104)]
    public int dmBitsPerPel;

    [FieldOffset(108)]
    public int dmPelsWidth;

    [FieldOffset(112)]
    public int dmPelsHeight;

    [FieldOffset(116)]
    public int dmDisplayFlags;

    [FieldOffset(116)]
    public int dmNup;

    [FieldOffset(120)]
    public int dmDisplayFrequency;
}

// See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183569(v=vs.85).aspx
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
internal struct DisplayDevice
{
    [MarshalAs(UnmanagedType.U4)]
    public int cb;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceString;

    [MarshalAs(UnmanagedType.U4)]
    public DisplayDeviceStateFlags StateFlags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceID;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceKey;
}

// See: https://msdn.microsoft.com/de-de/library/windows/desktop/dd162807(v=vs.85).aspx
[StructLayout(LayoutKind.Sequential)]
internal struct Pointl
{
    long x;
    long y;
}

internal enum DispChange : int
{
    Successful = 0,
    Restart = 1,
    Failed = -1,
    BadMode = -2,
    NotUpdated = -3,
    BadFlags = -4,
    BadParam = -5,
    BadDualView = -6
}

// http://www.pinvoke.net/default.aspx/Enums/DisplayDeviceStateFlags.html
[Flags()]
internal enum DisplayDeviceStateFlags : int
{
    /// <summary>The device is part of the desktop.</summary>
    AttachedToDesktop = 0x1,
    MultiDriver = 0x2,

    /// <summary>The device is part of the desktop.</summary>
    PrimaryDevice = 0x4,

    /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
    MirroringDriver = 0x8,

    /// <summary>The device is VGA compatible.</summary>
    VgaCompatible = 0x10,

    /// <summary>The device is removable; it cannot be the primary display.</summary>
    Removable = 0x20,

    /// <summary>The device has more display modes than its output devices support.</summary>
    ModesPruned = 0x8000000,
    Remote = 0x4000000,
    Disconnect = 0x2000000
}

// http://www.pinvoke.net/default.aspx/user32/ChangeDisplaySettingsFlags.html
[Flags()]
internal enum DisplaySettingsFlags : int
{
    CdsNone = 0,
    CdsUpdateregistry = 0x00000001,
    CdsTest = 0x00000002,
    CdsFullscreen = 0x00000004,
    CdsGlobal = 0x00000008,
    CdsSetPrimary = 0x00000010,
    CdsVideoparameters = 0x00000020,
    CdsEnableUnsafeModes = 0x00000100,
    CdsDisableUnsafeModes = 0x00000200,
    CdsReset = 0x40000000,
    CdsResetEx = 0x20000000,
    CdsNoreset = 0x10000000
}

[Flags()]
internal enum Dm : int
{
    Orientation = 0x00000001,
    PaperSize = 0x00000002,
    PaperLength = 0x00000004,
    PaperWidth = 0x00000008,
    Scale = 0x00000010,
    Position = 0x00000020,
    Nup = 0x00000040,
    DisplayOrientation = 0x00000080,
    Copies = 0x00000100,
    DefaultSource = 0x00000200,
    PrintQuality = 0x00000400,
    Color = 0x00000800,
    Duplex = 0x00001000,
    YResolution = 0x00002000,
    TtOption = 0x00004000,
    Collate = 0x00008000,
    FormName = 0x00010000,
    LogPixels = 0x00020000,
    BitsPerPixel = 0x00040000,
    PelsWidth = 0x00080000,
    PelsHeight = 0x00100000,
    DisplayFlags = 0x00200000,
    DisplayFrequency = 0x00400000,
    IcmMethod = 0x00800000,
    IcmIntent = 0x01000000,
    MediaType = 0x02000000,
    DitherType = 0x04000000,
    PanningWidth = 0x08000000,
    PanningHeight = 0x10000000,
    DisplayFixedOutput = 0x20000000
}
