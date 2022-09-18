using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SettingsForTV;

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

public class WindowsController : IDisposable
{
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;

    private readonly IntPtr _firstMonitorHandle;
    private readonly uint _maxValue;
    private readonly uint _minValue;


    private readonly uint _physicalMonitorsCount;

    private readonly List<IntPtr> results = new();
    private uint _currentValue;
    private PHYSICAL_MONITOR[] _physicalMonitorArray;


    public WindowsController(IntPtr windowHandle)
    {
        const uint dwFlags = 0u;
        var ptr = MonitorFromWindow(windowHandle, dwFlags);
        if (!GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref _physicalMonitorsCount))
            throw new Exception("Cannot get monitor count!");
        _physicalMonitorArray = new PHYSICAL_MONITOR[_physicalMonitorsCount];

        if (!GetPhysicalMonitorsFromHMONITOR(ptr, _physicalMonitorsCount, _physicalMonitorArray))
            throw new Exception("Cannot get physical monitor handle!");
        _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;

        if (!GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            throw new Exception("Cannot get monitor brightness!");
    }

    public WindowsController()
    {
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
    public static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize,
        ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
        ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize,
        [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness,
        ref uint currentBrightness, ref uint maxBrightness);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);


    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    public List<IntPtr> GetWindowHandles()
    {
        var windowHandles = new List<IntPtr>();
        foreach (var window in Process.GetProcesses())
        {
            window.Refresh();

            if (window.MainWindowHandle != IntPtr.Zero) windowHandles.Add(window.MainWindowHandle);
        }

        return windowHandles;
    }

    public void SetBrightness(int newValue) // 0 ~ 100
    {
        newValue = Math.Min(newValue, Math.Max(0, newValue));
        _currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
        SetMonitorBrightness(_firstMonitorHandle, _currentValue);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (_physicalMonitorsCount > 0)
            DestroyPhysicalMonitors(_physicalMonitorsCount, ref _physicalMonitorArray);
    }
}