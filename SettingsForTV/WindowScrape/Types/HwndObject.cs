using System.Drawing;
using SettingsForTV.WindowScrape.Constants;
using SettingsForTV.WindowScrape.Static;

namespace SettingsForTV.WindowScrape.Types;

public class HwndObject
{
    public HwndObject(IntPtr hwnd)
    {
        Hwnd = hwnd;
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

    public override string ToString()
    {
        var location = Location;
        var size = Size;
        return $"({Hwnd}) {location.X},{location.Y}:{size.Width}x{size.Height} \"{Title}\"";
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
}