using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Types;

namespace SettingsForTV.WindowScrape.Static;

internal static class HwndInterface
{
    public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

    public static bool ActivateWindow(IntPtr hwnd)
    {
        return SetForegroundWindow(hwnd);
    }

    public static void ClickHwnd(IntPtr hwnd)
    {
        SendMessage(hwnd, 0xf5, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("user32.dll")]
    private static extern bool CloseWindow(IntPtr hWnd);

    public static List<IntPtr> EnumChildren(IntPtr hwnd)
    {
        var zero = IntPtr.Zero;
        var list = new List<IntPtr>();
        do
        {
            zero = FindWindowEx(hwnd, zero, null, null);
            if (zero != IntPtr.Zero) list.Add(zero);
        } while (zero != IntPtr.Zero);

        return list;
    }

    public static List<IntPtr> EnumHwnds()
    {
        return EnumChildren(IntPtr.Zero);
    }

    [DllImport("user32.dll")]
    private static extern int FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className,
        string windowTitle);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    public static IntPtr GetHwnd(string windowText, string className)
    {
        return (IntPtr)FindWindow(className, windowText);
    }

    public static IntPtr GetHwndChild(IntPtr hwnd, string clsName, string ctrlText)
    {
        return FindWindowEx(hwnd, IntPtr.Zero, clsName, ctrlText);
    }

    public static string GetHwndClassName(IntPtr hwnd)
    {
        var lpClassName = new StringBuilder(0x100);
        GetClassName(hwnd, lpClassName, lpClassName.MaxCapacity);
        return lpClassName.ToString();
    }

    public static IntPtr GetHwndFromClass(string className)
    {
        return (IntPtr)FindWindow(className, null);
    }

    public static IntPtr GetHwndFromTitle(string windowText)
    {
        return (IntPtr)FindWindow(null, windowText);
    }

    public static IntPtr GetHwndParent(IntPtr hwnd)
    {
        return GetParent(hwnd);
    }

    public static Point GetHwndPos(IntPtr hwnd)
    {
        var lpRect = new Rect();
        GetWindowRect(hwnd, out lpRect);
        return new Point(lpRect.Left, lpRect.Top);
    }

    public static Size GetHwndSize(IntPtr hwnd)
    {
        var lpRect = new Rect();
        GetWindowRect(hwnd, out lpRect);
        return new Size(lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
    }

    public static string GetHwndText(IntPtr hwnd)
    {
        var capacity = (int)SendMessage(hwnd, 14, 0, 0) + 1;
        var lParam = new StringBuilder(capacity);
        SendMessage(hwnd, 13, (uint)capacity, lParam);
        return lParam.ToString();
    }

    public static string GetHwndTitle(IntPtr hwnd)
    {
        var lpString = new StringBuilder(GetHwndTitleLength(hwnd) + 1);
        GetWindowText(hwnd, lpString, lpString.Capacity);
        return lpString.ToString();
    }

    public static int GetHwndTitleLength(IntPtr hwnd)
    {
        return GetWindowTextLength(hwnd);
    }

    public static int GetMessageInt(IntPtr hwnd, WM msg)
    {
        return (int)SendMessage(hwnd, (uint)msg, 0, 0);
    }

    public static string GetMessageString(IntPtr hwnd, WM msg, uint param)
    {
        var lParam = new StringBuilder(0x10000);
        SendMessage(hwnd, (uint)msg, param, lParam);
        return lParam.ToString();
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr GetParent(IntPtr hWnd);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.Dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

    public static bool MinimizeWindow(IntPtr hwnd)
    {
        return CloseWindow(hwnd);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, string lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, StringBuilder lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    public static int SendMessage(IntPtr hwnd, WM msg, uint param1, string param2)
    {
        return (int)SendMessage(hwnd, (uint)msg, param1, param2);
    }

    public static int SendMessage(IntPtr hwnd, WM msg, uint param1, uint param2)
    {
        return (int)SendMessage(hwnd, (uint)msg, param1, param2);
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

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

    public static bool IsShown(IntPtr hWnd, int nCmdShow)
    {
        return ShowWindow(hWnd, nCmdShow);
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

    public static Size GetDisplayResolution()
    {
        var sf = GetWindowsScreenScalingFactor(false);
        var screenWidth = Screen.PrimaryScreen.Bounds.Width * sf;
        var screenHeight = Screen.PrimaryScreen.Bounds.Height * sf;
        return new Size((int)screenWidth, (int)screenHeight);
    }

    public static double GetWindowsScreenScalingFactor(bool percentage = true)
    {
        //Create Graphics object from the current windows handle
        var graphicsObject = Graphics.FromHwnd(IntPtr.Zero);
        //Get Handle to the device context associated with this Graphics object
        var deviceContextHandle = graphicsObject.GetHdc();
        //Call GetDeviceCaps with the Handle to retrieve the Screen Height
        var logicalScreenHeight = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.VERTRES);
        var physicalScreenHeight = GetDeviceCaps(deviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
        //Divide the Screen Heights to get the scaling factor and round it to two decimals
        var screenScalingFactor = Math.Round(physicalScreenHeight / (double)logicalScreenHeight, 2);
        //If requested as percentage - convert it
        if (percentage) screenScalingFactor *= 100.0;
        //Release the Handle and Dispose of the GraphicsObject object
        graphicsObject.ReleaseHdc(deviceContextHandle);
        graphicsObject.Dispose();
        //Return the Scaling Factor
        return screenScalingFactor;
    }

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    [DllImport("USER32.DLL")]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);


    #region AlignTop

    public static bool AlignTopCenter(IntPtr ptr, IntPtr position)
    {
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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
        var rct = new Rect();
        GetWindowRect(ptr, out rct);
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