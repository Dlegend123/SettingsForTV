using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Static;

namespace SettingsForTV.WindowScrape.Types;

public class HwndObject
{
    private readonly List<IntPtr> results = new();

    public HwndObject(IntPtr hwnd)
    {
        Hwnd = hwnd;
    }

    public HwndObject()
    {
    }

    public string ClassName => HwndInterface.GetHwndClassName(Hwnd);

    public IntPtr Hwnd { get; }

    public Point Location
    {
        get => HwndInterface.GetHwndPos(Hwnd);
        set => HwndInterface.SetHwndPos(Hwnd, value.X, value.Y);
    }

    public Size Size
    {
        get => HwndInterface.GetHwndSize(Hwnd);
        set => HwndInterface.SetHwndSize(Hwnd, value.Width, value.Height);
    }

    public string Text
    {
        get => HwndInterface.GetHwndText(Hwnd);
        set => HwndInterface.SetHwndText(Hwnd, value);
    }

    public string Title
    {
        get => HwndInterface.GetHwndTitle(Hwnd);
        set => HwndInterface.SetHwndTitle(Hwnd, value);
    }

    public void Click()
    {
        HwndInterface.ClickHwnd(Hwnd);
    }

    // <summary>
    // Bring this window to the foreground
    // </summary>
    public bool Activate()
    {
        return HwndInterface.ActivateWindow(Hwnd);
    }

    // <summary>
    // Minimize this window
    // </summary>
    public bool Minimize()
    {
        return HwndInterface.MinimizeWindow(Hwnd);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == typeof(HwndObject) && Equals((HwndObject)obj);
    }

    public bool Equals(HwndObject obj)
    {
        if (obj is null) return false;
        return ReferenceEquals(this, obj) || obj.Hwnd.Equals(Hwnd);
    }

    public HwndObject GetChild(string cls, string title)
    {
        return new HwndObject(HwndInterface.GetHwndChild(Hwnd, cls, title));
    }

    public List<HwndObject> GetChildren()
    {
        var list = new List<HwndObject>();
        foreach (var ptr in HwndInterface.EnumChildren(Hwnd)) list.Add(new HwndObject(ptr));
        return list;
    }

    public override int GetHashCode()
    {
        return Hwnd.GetHashCode();
    }

    public int GetMessageInt(WM msg)
    {
        return HwndInterface.GetMessageInt(Hwnd, msg);
    }

    public string GetMessageString(WM msg, uint param)
    {
        return HwndInterface.GetMessageString(Hwnd, msg, param);
    }

    public HwndObject GetParent()
    {
        return new HwndObject(HwndInterface.GetHwndParent(Hwnd));
    }

    public static HwndObject GetWindowByTitle(string title)
    {
        return new HwndObject(HwndInterface.GetHwndFromTitle(title));
    }

    public static HwndObject GetWindowByClassName(string className)
    {
        return new HwndObject(HwndInterface.GetHwndFromClass(className));
    }

    public static List<HwndObject> GetWindows()
    {
        return HwndInterface.EnumHwnds().Select(ptr => new HwndObject(ptr)).ToList();
    }

    public static bool operator ==(HwndObject a, HwndObject b)
    {
        if (a is null)
            return b is null;
        if (b is null) return a is null;
        return a.Hwnd == b.Hwnd;
    }

    public static bool operator !=(HwndObject a, HwndObject b)
    {
        return !(a == b);
    }

    public void SendMessage(WM msg, uint param1, string param2)
    {
        HwndInterface.SendMessage(Hwnd, msg, param1, param2);
    }

    public void SendMessage(WM msg, uint param1, uint param2)
    {
        HwndInterface.SendMessage(Hwnd, msg, param1, param2);
    }

    private int WindowEnum(IntPtr hWnd, int lParam)
    {
        var threadId = HwndInterface.GetWindowThreadProcessId(hWnd, out _);
        if (threadId == lParam) results.Add(hWnd);

        return 1;
    }

    public IntPtr[] GetWindowHandlesForThread(int threadHandle)
    {
        results.Clear();
        EnumWindows(WindowEnum, threadHandle);

        return results.ToArray();
    }

    public bool IsProcessWindowed(Process externalProcess)
    {
        return externalProcess.MainWindowHandle != IntPtr.Zero ||
               (from ProcessThread threadInfo in externalProcess.Threads
                   select GetWindowHandlesForThread(threadInfo.Id)
                   into windows
                   where windows != null
                   from handle in windows
                   select handle).Any(HwndInterface.WindowVisible);
    }

    private static bool EnumWindow(IntPtr handle, IntPtr pointer)
    {
        var gch = GCHandle.FromIntPtr(pointer);
        if (gch.Target is not List<IntPtr> list)
            throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
        list.Add(handle);
        //  You can modify this to check to see if you want to cancel the operation, then return a null here
        return true;
    }

    private IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
    {
        var result = new List<IntPtr>();
        var listHandle = GCHandle.Alloc(result);
        try
        {
            var childProc = new HwndInterface.Win32Callback(EnumWindow);
            HwndInterface.GetEnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
        }
        finally
        {
            if (listHandle.IsAllocated)
                listHandle.Free();
        }

        return result;
    }


    public void ShowAllOpenWindows()
    {
        var resolution = HwndInterface.GetDisplayResolution();

        var windows = Process.GetProcesses().Where(IsProcessWindowed).ToList();

        var columnWidth = resolution.Width / windows.Count;
        var rowHeight = resolution.Height / windows.Count;

        for (var i = 0; i < windows.Count; i++)
        {
            decimal index = i;
            var x = Math.Round(index / 3 * columnWidth, 0);
            var y = Math.Round(index / 2 * rowHeight, 0);

            windows[i].Refresh();
            HwndInterface.IsShown(windows[i].MainWindowHandle, (int)PositioningFlags.SW_SHOWNORMAL);
            HwndInterface.MoveWindow(windows[i].MainWindowHandle, decimal.ToInt32(x), decimal.ToInt32(y), columnWidth,
                rowHeight,
                true);
        }
    }

    public override string ToString()
    {
        var location = Location;
        var size = Size;
        return $"({Hwnd}) {location.X},{location.Y}:{size.Width}x{size.Height} \"{Title}\"";
    }

    [DllImport("user32.Dll")]
    private static extern int EnumWindows(EnumWindowsProc x, int y);

    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);
}