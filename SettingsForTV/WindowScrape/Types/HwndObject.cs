using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Static;
using static SettingsForTV.WindowScrape.Static.HwndInterface;

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
        return HwndInterface.GetMessageInt(Hwnd, msg);
    }

    public string GetMessageString(WM msg, uint param)
    {
        return HwndInterface.GetMessageString(Hwnd, msg, param);
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
        var threadId = GetWindowThreadProcessId(hWnd, out _);
        if (threadId == lParam) results.Add(hWnd);

        return 1;
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

    public static bool IsNotSystemProcess(Process process)
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

    public void ShowAllOpenWindows()
    {
        var resolution = GetDisplayResolution();

        var windows = Process.GetProcesses().Where(IsProcessWindowed).Where(IsNotSystemProcess).ToList();

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

        var iRet = ChangeDisplaySettings(ref dm, CDS_TEST);

        if (iRet == DISP_CHANGE_FAILED)
        {
            MessageBox.Show("Unable to process your request");
            MessageBox.Show("Description: Unable To Process Your Request. Sorry For This Inconvenience.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            iRet = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);

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
}