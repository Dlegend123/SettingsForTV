using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SettingsForTV.WindowScrape.Static;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO.Compression;

namespace SettingsForTV.WindowScrape
{
    public class Wallpaper
    {
        Wallpaper()
        {
        }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        public enum Style
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }

        public static void Set(Uri uri, Style style)
        {
            var client = new HttpClient();
            _ = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };

            client.DefaultRequestHeaders.UserAgent.TryParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(9000);
            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new StringWithQualityHeaderValue("gzip"));

            //var s = new WebClient().OpenRead(uri.ToString());
            var responseStream = new GZipStream(client.GetStreamAsync(uri).Result, CompressionMode.Decompress);
            var reader = new StreamReader(responseStream);
            var img = Image.FromStream(reader.BaseStream);
            //var img = Image.FromStream(s);
            var tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, ImageFormat.Bmp);

            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            

            if (key != null)
            {
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
            }

            _ = HwndInterface.SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}

