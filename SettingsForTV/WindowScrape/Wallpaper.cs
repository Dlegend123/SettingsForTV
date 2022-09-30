using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using static SettingsForTV.WindowScrape.Static.HwndInterface;
using static SettingsForTV.WindowScrape.Types.HwndObject;

namespace SettingsForTV.WindowScrape;

public class Wallpaper
{
    public enum Style
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center
    }

    private Wallpaper()
    {
    }

    public static bool PaintWall(string wallFilePath, Style style)
    {
        var primaryFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var destWallFilePath = Path.Combine(primaryFolder + @"\Microsoft\Windows\Themes", "rollerWallpaper.bmp");

        Bitmap imgTemp = null;
        try
        {
            var img = Image.FromFile(Path.GetFullPath(wallFilePath));
            imgTemp = new Bitmap(img);
            imgTemp.Save(destWallFilePath, ImageFormat.Bmp);
            Console.WriteLine("Wallpaper saved to primary path: " + destWallFilePath);
        }
        catch (Exception e1)
        {
            Console.WriteLine(e1);
            try
            {
                var secondaryFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                destWallFilePath = Path.Combine(secondaryFolder, "rollerWallpaper.bmp");
                if (imgTemp != null)
                {
                    imgTemp.Save(destWallFilePath, ImageFormat.Bmp);
                    Console.WriteLine("Wallpaper saved to secondary path: " + destWallFilePath);
                }
                else
                {
                    Console.WriteLine("Failed to save wallpaper to secondary path: " + destWallFilePath);
                    return false;
                }
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
                return false;
            }
        }

        try
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (key == null) return false;
            switch (style)
            {
                case Style.Fill:
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    break;
                case Style.Fit:
                    key.SetValue(@"WallpaperStyle", 6.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    break;
                // Windows 8 or newer only!
                case Style.Span:
                    key.SetValue(@"WallpaperStyle", 22.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    break;
                case Style.Stretch:
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    break;
                case Style.Tile:
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                    break;
                case Style.Center:
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }

            return SetSystemWallpaper(destWallFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public static bool EnableDpiAwareness()
    {
        try
        {
            if (Environment.OSVersion.Version.Major < 6)
                return false;
            _ = SetProcessDpiAwareness(ProcessDpiAwareness.ProcessPerMonitorDpiAware);
            return true;
        }
        catch (Exception e1)
        {
            Console.WriteLine(e1);
            return false;
        }
    }

    public static List<WallTuple> GetWallTuples(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            return null;

        // 'something_1920x1080.jpg' will match
        var wallNameRegex = new Regex(@"[_\-\.][0-9]{3,4}x[0-9]{3,4}\.(bmp|jpg|jpeg|png)$");

        // '1920x1080' will match
        var resolutionRegex = new Regex(@"[0-9]{3,4}x[0-9]{3,4}");

        var wallFileList = Directory.GetFiles(path).Where(x => wallNameRegex.IsMatch(x)).ToList();
        var wallTupleList =
            from wallFilePath in wallFileList
            let resExp = wallNameRegex.Match(wallFilePath).Groups[0].Value
            let xExp = int.Parse(resolutionRegex.Match(resExp).Groups[0].Value.Split('x')[0])
            let yExp = int.Parse(resolutionRegex.Match(resExp).Groups[0].Value.Split('x')[1])
            select new WallTuple
            {
                fullPath = wallFilePath,
                xRes = xExp,
                yRes = yExp
            };

        return wallTupleList.ToList();
    }

    public struct WallTuple
    {
        public string fullPath;
        public int xRes;
        public int yRes;
    }
}