using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Static;
using static SettingsForTV.WindowScrape.Static.HwndInterface;

namespace SettingsForTV.WindowScrape.Types;

public class HwndObject
{
    public uint _currentValue;
    public IntPtr _firstMonitorHandle;
    public uint _maxValue;
    public uint _minValue;
    public PHYSICAL_MONITOR[] _physicalMonitorArray;
    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    public enum ProcessDpiAwareness
    {
        ProcessDpiUnaware = 0,
        ProcessSystemDpiAware = 1,
        ProcessPerMonitorDpiAware = 2
    }

    private uint _physicalMonitorsCount;

    private readonly List<IntPtr> results = new();

    public HwndObject(IntPtr hwnd)
    {
        Hwnd = hwnd;
    }

    public HwndObject()
    {
    }

    public string ClassName => GetHwndClassName(Hwnd);

    public IntPtr Hwnd { get; }

    public Point Location
    {
        get => GetHwndPos(Hwnd);
        set => SetHwndPos(Hwnd, value.X, value.Y);
    }

    public Size Size
    {
        get => GetHwndSize(Hwnd);
        set => SetHwndSize(Hwnd, value.Width, value.Height);
    }

    public string Text
    {
        get => GetHwndText(Hwnd);
        set => SetHwndText(Hwnd, value);
    }

    public string Title
    {
        get => GetHwndTitle(Hwnd);
        set => SetHwndTitle(Hwnd, value);
    }

    public void Click()
    {
        ClickHwnd(Hwnd);
    }

    // <summary>
    // Bring this window to the foreground
    // </summary>
    public bool Activate()
    {
        return ActivateWindow(Hwnd);
    }

    // <summary>
    // Minimize this window
    // </summary>
    public bool Minimize()
    {
        return MinimizeWindow(Hwnd);
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
        return new HwndObject(GetHwndChild(Hwnd, cls, title));
    }

    public List<HwndObject> GetChildren()
    {
        var list = new List<HwndObject>();
        foreach (var ptr in EnumChildren(Hwnd)) list.Add(new HwndObject(ptr));
        return list;
    }

    public override int GetHashCode()
    {
        return Hwnd.GetHashCode();
    }

    public int GetMessageInt(WM msg)
    {
        return GetMessageInt(Hwnd, msg);
    }

    public string GetMessageString(WM msg, uint param)
    {
        return GetMessageString(Hwnd, msg, param);
    }

    public HwndObject GetParent()
    {
        return new HwndObject(GetHwndParent(Hwnd));
    }

    public static HwndObject GetWindowByTitle(string title)
    {
        return new HwndObject(GetHwndFromTitle(title));
    }

    public static HwndObject GetWindowByClassName(string className)
    {
        return new HwndObject(GetHwndFromClass(className));
    }

    public static List<HwndObject> GetWindows()
    {
        return EnumHwnds().Select(ptr => new HwndObject(ptr)).ToList();
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

    private bool WindowEnum(IntPtr hWnd, int lParam)
    {
        var threadId = GetWindowThreadProcessId(hWnd, out _);
        if (threadId == lParam) results.Add(hWnd);

        return true;
    }

    public IntPtr[] GetWindowHandlesForThread(int threadHandle)
    {
        results.Clear();
        _ = EnumWindows(WindowEnum, threadHandle);

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
                   select handle).Any(WindowVisible);
    }

    public bool IsNotSystemProcess(Process process)
    {
        try
        {
            _ = GetSecurityInfo(process.Handle, SE_OBJECT_TYPE.SE_KERNEL_OBJECT,
                SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION, out var ownerSid, out _, out _, out _, out _);

            var criticalProcess = false;

            IsProcessCritical(process.Handle, ref criticalProcess);

            return !new SecurityIdentifier(ownerSid).IsWellKnown(WellKnownSidType.LocalSystemSid) || !criticalProcess;

            // The process is running under the local system account.
            // This is a critical process, it should be listed
            // in the "Windows processes" section.
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool EnumWindow(IntPtr handle, IntPtr pointer)
    {
        var gch = GCHandle.FromIntPtr(pointer);
        try
        {
            if (gch.Target is not List<IntPtr> list)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
        }
        catch (Exception)
        {
            return false;
        }

        //  You can modify this to check to see if you want to cancel the operation, then return a null here
        return true;
    }

    private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
    {
        var result = new List<IntPtr>();
        var listHandle = GCHandle.Alloc(result);
        try
        {
            var childProc = new Win32Callback(EnumWindow);
            GetEnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
        }
        finally
        {
            if (listHandle.IsAllocated)
                listHandle.Free();
        }

        return result;
    }

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

    private static IEnumerable<IntPtr> GetRootWindowsOfProcess(int pid)
    {
        var rootWindows = GetChildWindows(IntPtr.Zero);
        var dsProcRootWindows = new List<IntPtr>();
        foreach (var hWnd in rootWindows)
        {
            _ = GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
            if (lpdwProcessId == pid)
                dsProcRootWindows.Add(hWnd);
        }

        return dsProcRootWindows;
    }

    public void ShowAllOpenWindows()
    {
        var resolution = GetDisplayResolution();

        var windows = Process.GetProcesses().Where(process => !string.IsNullOrEmpty(process.MainWindowTitle))
            .Where(IsProcessWindowed).Where(IsNotSystemProcess).ToList();

        var columnWidth = resolution.Width / windows.Count;
        var rowHeight = resolution.Height / windows.Count;

        for (var i = 0; i < windows.Count; i++)
        {
            decimal index = i;
            var x = Math.Round(index / 3 * columnWidth, 0);
            var y = Math.Round(index / 2 * rowHeight, 0);

            windows[i].Refresh();
            ShowWindow(windows[i].MainWindowHandle, (int)PositioningFlags.SW_SHOWNORMAL);
            MoveWindow(windows[i].MainWindowHandle, decimal.ToInt32(x), decimal.ToInt32(y), columnWidth,
                rowHeight,
                true);
        }
    }

    public void GetAllOpenWindows(Process process)
    {
        var resolution = GetDisplayResolution();


        process.Refresh();
        windows.AddRange(GetRootWindowsOfProcess(process.Id));

        });
        var columnWidth = resolution.Width / windows.Count;
        var rowHeight = resolution.Height / windows.Count;

        for (var i = 0; i < windows.Count; i++)
        {
            decimal index = i;
            var x = Math.Round(index / 3 * columnWidth, 0);
            var y = Math.Round(index / 2 * rowHeight, 0);

            ShowWindow(windows[i], (int)PositioningFlags.SW_SHOWNORMAL);
            MoveWindow(windows[i], decimal.ToInt32(x), decimal.ToInt32(y), columnWidth,
                rowHeight,
                true);
        }
    }

    public static List<IntPtr> GetOpenWindows()
    {
        var shellWindow = GetShellWindow();
        var windows = new List<IntPtr>();

        EnumWindows(delegate(IntPtr hWnd, int lParam)
        {
            if (hWnd == shellWindow) return true;
            if (!IsWindowVisible(hWnd)) return true;

            windows.Add(hWnd);
            return true;
        }, 0);

        return windows;
    }

    public void SetBrightness(int newValue) // 0 ~ 100
    {
        newValue = Math.Min(newValue, Math.Max(0, newValue));
        _currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
        SetMonitorBrightness(_firstMonitorHandle, _currentValue);
    }

    public static void SetResolution(int iWidth, int iHeight)
    {
        var dm = new DEVMODE1
        {
            dmDeviceName = new string(new char[32]),
            dmFormName = new string(new char[32])
        };
        dm.dmSize = (short)Marshal.SizeOf(dm);

        if (!EnumDisplaySettingsA(null, ENUM_CURRENT_SETTINGS, ref dm))
            MessageBox.Show("Description: Failed To Change The Resolution.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        dm.dmPelsWidth = iWidth;
        dm.dmPelsHeight = iHeight;

        var iRet = ChangeDisplaySettingsA(ref dm, CDS_TEST);

        if (iRet == DISP_CHANGE_FAILED)
        {
            MessageBox.Show("Unable to process your request");
            MessageBox.Show("Description: Unable To Process Your Request. Sorry For This Inconvenience.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            iRet = ChangeDisplaySettingsA(ref dm, CDS_UPDATEREGISTRY);

            switch (iRet)
            {
                case DISP_CHANGE_SUCCESSFUL:
                {
                    MessageBox.Show(
                        "Description: Success.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                    //successfull change
                }
                case DISP_CHANGE_RESTART:
                {
                    MessageBox.Show(
                        "Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                    //windows 9x series you have to restart
                }
                default:
                {
                    MessageBox.Show("Description: Failed To Change The Resolution.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                    //failed to change
                }
            }
        }
    }

    public override string ToString()
    {
        var location = Location;
        var size = Size;
        return $"({Hwnd}) {location.X},{location.Y}:{size.Width}x{size.Height} \"{Title}\"";
    }

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
        _ = GetClassName(hwnd, lpClassName, lpClassName.MaxCapacity);
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
        return HwndInterface.GetParent(hwnd);
    }

    public static Point GetHwndPos(IntPtr hwnd)
    {
        GetWindowRect(hwnd, out var lpRect);
        return new Point(lpRect.Left, lpRect.Top);
    }

    public static Size GetHwndSize(IntPtr hwnd)
    {
        GetWindowRect(hwnd, out var lpRect);
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
        _ = GetWindowText(hwnd, lpString, lpString.Capacity);
        return lpString.ToString();
    }

    public static int GetHwndTitleLength(IntPtr hwnd)
    {
        return GetWindowTextLengthA(hwnd);
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

    public bool GetBrightness(IntPtr windowHandle)
    {
        const uint dwFlags = 0u;
        var ptr = MonitorFromWindow(windowHandle, dwFlags);
        var gotNumMonitors = GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref _physicalMonitorsCount);
        if (!gotNumMonitors)
            return false;
        //MessageBox.Show("Cannot get monitor count!");
        else
        {
            _physicalMonitorArray = new PHYSICAL_MONITOR[_physicalMonitorsCount];
            var gotPhysicalMonitors =
                GetPhysicalMonitorsFromHMONITOR(ptr, _physicalMonitorsCount, _physicalMonitorArray);
            if (!gotPhysicalMonitors)
                return false;
            //MessageBox.Show("Cannot get physical monitor handle!");
            else
            {
                _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;

                if (!GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
                    return false;
                //MessageBox.Show("Cannot get monitor brightness!");
            }
        }

        return true;
    }
    public static bool SetSystemWallpaper(string wallpaperFilePath)
    {
        try
        {
            _ = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperFilePath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            return true;
        }
        catch
        {
            return false;
        }
    }
}