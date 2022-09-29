using SettingsForTV.WindowScrape.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SettingsForTV.WindowScrape.Static;
using System.Windows.Interop;

namespace SettingsForTV
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public Overlay()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Topmost = true;

            // Get this window's handle
            var hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            // Intentionally do not await the result
            Application.Current.Dispatcher.BeginInvoke(new Action(async () => await RetrySetTopMost(hwnd)));
        }

        private void ShowAllWindows_Click(object sender, RoutedEventArgs e)
        {
            var b = new HwndObject();
            b.ShowAllOpenWindows();
            //   var d = Process.GetProcesses().Where(b.IsProcessWindowed).ToList();
            //HwndObject.SetResolution(1920,1080);

            // var q = HwndInterface.AlignBottomCenter(d[1].MainWindowHandle, new IntPtr(-1));
        }
        // The code below will retry several times before giving up. This always worked with one retry in my tests.
        internal const int GWL_EXSTYLE = -20;
        internal const int WS_EX_TOPMOST = 0x00000008;
        private const int RetrySetTopMostDelay = 200;
        private const int RetrySetTopMostMax = 20;

        private static async Task RetrySetTopMost(IntPtr hwnd)
        {
            for (var i = 0; i < RetrySetTopMostMax; i++)
            {
                await Task.Delay(RetrySetTopMostDelay);
                var winStyle = HwndInterface.GetWindowLong(hwnd, GWL_EXSTYLE);

                if ((winStyle & WS_EX_TOPMOST) != 0) break;

                Application.Current.MainWindow.Topmost = false;
                Application.Current.MainWindow.Topmost = true;
            }
        }
    }
}
