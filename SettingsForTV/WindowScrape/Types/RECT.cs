using System.Runtime.InteropServices;

namespace SettingsForTV.WindowScrape.Types;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}