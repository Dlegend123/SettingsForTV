using SettingsForTV.WindowScrape.Static;
using SettingsForTV.WindowScrape.Types;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SettingsForTV;

/// <summary>
///     Interaction logic for Overlay.xaml
/// </summary>
public partial class Overlay : Window
{
    // The code below will retry several times before giving up. This always worked with one retry in my tests.
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_TOPMOST = 0x00000008;
    private const int RetrySetTopMostDelay = 200;
    private const int RetrySetTopMostMax = 20;
    SettingsWindow? settingsWindow;
    private int identifierGeneration;
    private Settings settings;
    public Overlay()
    {
        InitializeComponent();
        settings = Settings.GetSettings();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        Topmost = true;
        Overlay_OnActivated(sender, e);
    }

    private void ShowAllWindows_Click(object sender, RoutedEventArgs e)
    {
        var b = new HwndObject();
        b.ShowAllOpenWindows();
        //   var d = Process.GetProcesses().Where(b.IsProcessWindowed).ToList();
        //HwndObject.SetResolution(1920,1080);

        // var q = HwndInterface.AlignBottomCenter(d[1].MainWindowHandle, new IntPtr(-1));
    }

    private static async Task RetrySetTopMost(IntPtr hwnd)
    {
        for (var i = 0; i < RetrySetTopMostMax; i++)
        {
            await Task.Delay(RetrySetTopMostDelay);
            var winStyle = HwndInterface.GetWindowLong(hwnd, GWL_EXSTYLE);

            if ((winStyle & WS_EX_TOPMOST) != 0) break;

            if (Application.Current.MainWindow == null) continue;
            Application.Current.MainWindow.Topmost = false;
            Application.Current.MainWindow.Topmost = true;
        }
    }

    private void Overlay_OnActivated(object? sender, EventArgs e)
    {
        Width = SystemParameters.PrimaryScreenWidth;

        Height = SystemParameters.PrimaryScreenHeight;

        Topmost = true;
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        if (settingsWindow == null)
        {
            settingsWindow = new SettingsWindow();
            identifierGeneration = GC.GetGeneration(settingsWindow);
            settingsWindow.Closed += (_, _) =>
            {
                settingsWindow = null;
                GC.Collect(((Overlay)Application.Current.MainWindow).identifierGeneration, GCCollectionMode.Forced);
                ((Overlay)Application.Current.MainWindow).settings = Settings.GetSettings();
            };
            settingsWindow.Show();
        }
        else
        {
            settingsWindow.ChangeWindowStateToNormal();
            Overlay_OnActivated(sender, e);
        }
    }
}