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
            // The Window was deactivated 
            Topmost= true;
        }

        private void ShowAllWindows_Click(object sender, RoutedEventArgs e)
        {
            var b = new HwndObject();
            b.ShowAllOpenWindows();
            //   var d = Process.GetProcesses().Where(b.IsProcessWindowed).ToList();
            //HwndObject.SetResolution(1920,1080);

            // var q = HwndInterface.AlignBottomCenter(d[1].MainWindowHandle, new IntPtr(-1));
        }
    }
}
