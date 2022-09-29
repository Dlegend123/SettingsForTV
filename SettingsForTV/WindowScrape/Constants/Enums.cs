﻿using System;

namespace SettingsForTV.WindowScrape.Constants;

/// <summary>
/// Window handles (HWND) used for hWndInsertAfter
/// </summary>
public static class Hwnd
{
    public static IntPtr
        NoTopMost = new IntPtr(-2),
        TopMost = new IntPtr(-1),
        Top = new IntPtr(0),
        Bottom = new IntPtr(1);
}

/// <summary>
/// SetWindowPos Flags
/// </summary>
public static class Swp
{
    public static readonly uint
        NOSIZE = 0x0001,
        NOMOVE = 0x0002,
        NOZORDER = 0x0004,
        NOREDRAW = 0x0008,
        NOACTIVATE = 0x0010,
        DRAWFRAME = 0x0020,
        FRAMECHANGED = 0x0020,
        SHOWWINDOW = 0x0040,
        HIDEWINDOW = 0x0080,
        NOCOPYBITS = 0x0100,
        NOOWNERZORDER = 0x0200,
        NOREPOSITION = 0x0200,
        NOSENDCHANGING = 0x0400,
        DEFERERASE = 0x2000,
        ASYNCWINDOWPOS = 0x4000;
}
[Flags]
internal enum PositioningFlags
{
    SW_HIDE = 0,
    SW_SHOWNORMAL = 1,
    SW_SHOWMINIMIZED = 2,
    SW_SHOWMAXIMIZED = 3,
    SW_SHOWNOACTIVATE = 4,
    SW_SHOW = 5,
    SW_MINIMIZE = 6,
    SW_SHOWMINNOACTIVE = 7,
    SW_SHOWNA = 8,
    SW_RESTORE = 9,
    SW_SHOWDEFAULT = 10,
    SW_FORCEMINIMIZE = 11
}

[Flags]
public enum WindowStyles : uint
{
    WS_OVERLAPPED = 0x00000000,
    WS_POPUP = 0x80000000,
    WS_CHILD = 0x40000000,
    WS_MINIMIZE = 0x20000000,
    WS_VISIBLE = 0x10000000,
    WS_DISABLED = 0x08000000,
    WS_CLIPSIBLINGS = 0x04000000,
    WS_CLIPCHILDREN = 0x02000000,
    WS_MAXIMIZE = 0x01000000,
    WS_BORDER = 0x00800000,
    WS_DLGFRAME = 0x00400000,
    WS_VSCROLL = 0x00200000,
    WS_HSCROLL = 0x00100000,
    WS_SYSMENU = 0x00080000,
    WS_THICKFRAME = 0x00040000,
    WS_GROUP = 0x00020000,
    WS_TABSTOP = 0x00010000,

    WS_MINIMIZEBOX = 0x00020000,
    WS_MAXIMIZEBOX = 0x00010000,

    WS_CAPTION = WS_BORDER | WS_DLGFRAME,
    WS_TILED = WS_OVERLAPPED,
    WS_ICONIC = WS_MINIMIZE,
    WS_SIZEBOX = WS_THICKFRAME,
    WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
    WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
    WS_CHILDWINDOW = WS_CHILD
}

[Flags]
public enum EXWindowStyles : uint
{
    //Extended Window Styles

    WS_EX_DLGMODALFRAME = 0x00000001,
    WS_EX_NOPARENTNOTIFY = 0x00000004,
    WS_EX_TOPMOST = 0x00000008,
    WS_EX_ACCEPTFILES = 0x00000010,
    WS_EX_TRANSPARENT = 0x00000020,
    WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
    //#if(WINVER >= 0x0400)

    WS_EX_MDICHILD = 0x00000040,
    WS_EX_TOOLWINDOW = 0x00000080,
    WS_EX_WINDOWEDGE = 0x00000100,
    WS_EX_CLIENTEDGE = 0x00000200,
    WS_EX_CONTEXTHELP = 0x00000400,

    WS_EX_RIGHT = 0x00001000,
    WS_EX_LEFT = 0x00000000,
    WS_EX_RTLREADING = 0x00002000,
    WS_EX_LTRREADING = 0x00000000,
    WS_EX_LEFTSCROLLBAR = 0x00004000,
    WS_EX_RIGHTSCROLLBAR = 0x00000000,

    WS_EX_CONTROLPARENT = 0x00010000,
    WS_EX_STATICEDGE = 0x00020000,
    WS_EX_APPWINDOW = 0x00040000,

    WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
    WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
    //#endif /* WINVER >= 0x0400 */

    //#if(WIN32WINNT >= 0x0500)

    WS_EX_LAYERED = 0x00080000,
    //#endif /* WIN32WINNT >= 0x0500 */

    //#if(WINVER >= 0x0500)

    WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
    WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
    //#endif /* WINVER >= 0x0500 */

    //#if(WIN32WINNT >= 0x0500)

    WS_EX_COMPOSITED = 0x02000000,

    WS_EX_NOACTIVATE = 0x08000000
    //#endif /* WIN32WINNT >= 0x0500 */
}

public enum DeviceCap
{
    VERTRES = 10,
    DESKTOPVERTRES = 117
}

public enum SE_OBJECT_TYPE
{
    SE_UNKNOWN_OBJECT_TYPE,
    SE_FILE_OBJECT,
    SE_SERVICE,
    SE_PRINTER,
    SE_REGISTRY_KEY,
    SE_LMSHARE,
    SE_KERNEL_OBJECT,
    SE_WINDOW_OBJECT,
    SE_DS_OBJECT,
    SE_DS_OBJECT_ALL,
    SE_PROVIDER_DEFINED_OBJECT,
    SE_WMIGUID_OBJECT,
    SE_REGISTRY_WOW64_32KEY
}

public enum SECURITY_INFORMATION
{
    OWNER_SECURITY_INFORMATION = 1,
    GROUP_SECURITY_INFORMATION = 2,
    DACL_SECURITY_INFORMATION = 4,
    SACL_SECURITY_INFORMATION = 8
}