using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Types;

namespace SettingsForTV.WindowScrape.Static;

public class HwndInterface
{
    public delegate bool EnumWindowsProc(IntPtr hwnd, int lParam);

    public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

    public const int ENUM_CURRENT_SETTINGS = -1;
    public const int CDS_UPDATEREGISTRY = 0x01;
    public const int CDS_TEST = 0x02;
    public const int DISP_CHANGE_SUCCESSFUL = 0;
    public const int DISP_CHANGE_RESTART = 1;
    public const int DISP_CHANGE_FAILED = -1;

    [DllImport("user32.dll")]
    public static extern bool CloseWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplaySettingsA(string deviceName, int modeNum, ref DEVMODE1 devMode);

    [DllImport("user32.dll")]
    public static extern int ChangeDisplaySettingsA(ref DEVMODE1 devMode, int flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className,
        string windowTitle);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.Dll")]
    public static extern bool EnumWindows(EnumWindowsProc x, int y);

    [DllImport("USER32.DLL")]
    public static extern IntPtr GetShellWindow();


    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool IsProcessCritical(IntPtr hProcess, ref bool Critical);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLengthA(IntPtr hWnd);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint GetSecurityInfo(IntPtr handle, SE_OBJECT_TYPE objectType,
        SECURITY_INFORMATION securityInfo, out IntPtr sidOwner, out IntPtr sidGroup, out IntPtr dacl, out IntPtr sacl,
        out IntPtr securityDescriptor);

    [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
    public static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize,
        [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
        ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness,
        ref uint currentBrightness, ref uint maxBrightness);

    [DllImport("user32.Dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, string lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, StringBuilder lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public static int SendMessage(IntPtr hwnd, WM msg, uint param1, string param2)
    {
        return (int)SendMessage(hwnd, (uint)msg, param1, param2);
    }

    public static int SendMessage(IntPtr hwnd, WM msg, uint param1, uint param2)
    {
        return (int)SendMessage(hwnd, (uint)msg, param1, param2);
    }

    public static bool MinimizeWindow(IntPtr hwnd)
    {
        return CloseWindow(hwnd);
    }

    public static bool SetHwndPos(IntPtr hwnd, int x, int y)
    {
        return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, 5);
    }

    public static bool WindowVisible(IntPtr hwnd)
    {
        return IsWindowVisible(hwnd);
    }

    public static bool GetEnumChildWindows(IntPtr parent, Win32Callback childProc, IntPtr listHandle)
    {
        return EnumChildWindows(parent, childProc, listHandle);
    }

    public static bool SetHwndSize(IntPtr hwnd, int w, int h)
    {
        return SetWindowPos(hwnd, IntPtr.Zero, 0, 0, w, h, 6);
    }

    public static void SetHwndText(IntPtr hwnd, string text)
    {
        SendMessage(hwnd, 12, 0, text);
    }

    public static bool SetHwndTitle(IntPtr hwnd, string text)
    {
        return SetWindowText(hwnd, text);
    }


    public static bool ActivateWindow(IntPtr hwnd)
    {
        return SetForegroundWindow(hwnd);
    }

    public static void ClickHwnd(IntPtr hwnd)
    {
        SendMessage(hwnd, 0xf5, IntPtr.Zero, IntPtr.Zero);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public Rectangle rcNormalPosition;
    }

    #region AlignTop

    public static bool AlignTopCenter(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(screen.Left + screen.Width / 2 - (rct.Right - rct.Left) / 2,
            0);
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignTopRight(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(screen.Width - (rct.Right - rct.Left), 0);
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignTopLeft(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out _);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var pt = new Point(0, 0);
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    #endregion

    #region AlignCenter

    public static bool AlignCenter(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(screen.Left + screen.Width / 2 - (rct.Right - rct.Left) / 2,
            screen.Top + screen.Height / 2 - (rct.Bottom - rct.Top) / 2);
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignCenterLeft(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(0, screen.Top + screen.Height / 2 - (rct.Bottom - rct.Top) / 2);
        return SetWindowPos(ptr, position, 0, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignCenterRight(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;

        var pt = new Point(screen.Width - (rct.Right - rct.Left),
            screen.Top + screen.Height / 2 - (rct.Bottom - rct.Top) / 2);
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    #endregion

    #region AlignBottom

    public static bool AlignBottomLeft(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(0, screen.Height - (rct.Bottom - rct.Top));
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignBottomRight(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(screen.Width - (rct.Right - rct.Left), screen.Height - (rct.Bottom - rct.Top));
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    public static bool AlignBottomCenter(IntPtr ptr, IntPtr position)
    {
        GetWindowRect(ptr, out var rct);
        if (ptr == IntPtr.Zero) return false;
        // Move the window to (0,0) without changing its size or position
        // in the Z order.
        var screen = Screen.FromHandle(ptr).Bounds;
        var pt = new Point(screen.Left + screen.Width / 2 - (rct.Right - rct.Left) / 2,
            screen.Height - (rct.Bottom - rct.Top));
        return SetWindowPos(ptr, position, pt.X, pt.Y, 0, 0,
            (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
    }

    #endregion
}