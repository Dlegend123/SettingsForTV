using System.Diagnostics;
using System.Linq;
using System.Windows;
using SettingsForTV.WindowScrape.Types;

namespace SettingsForTV;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        var b = new HwndObject();
        //   var d = Process.GetProcesses().Where(b.IsProcessWindowed).ToList();
        //HwndObject.SetResolution(1920,1080);

        // var q = HwndInterface.AlignBottomCenter(d[1].MainWindowHandle, new IntPtr(-1));
        InitializeComponent();
        var processes = Process.GetProcesses().Where(process => !string.IsNullOrEmpty(process.MainWindowTitle))
            .Where(b.IsProcessWindowed).Where(b.IsNotSystemProcess).ToList();
        foreach (var q in from x in processes select b.GetBrightness(x.MainWindowHandle))
        {
            var t = "";
        }
    }
}