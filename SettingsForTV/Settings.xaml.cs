using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SettingsForTV;

/// <summary>
///     Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : Window
{
    public Settings()
    {
        InitializeComponent();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void SettingsClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public void ChangeWindowStateToNormal()
    {
        WindowState = WindowState.Normal;
        Topmost = true;
        Topmost = false;
    }
    private void Settings_OnActivated(object? sender, EventArgs e)
    {
        Width = SystemParameters.PrimaryScreenWidth;

        Height = SystemParameters.PrimaryScreenHeight;

        Topmost = true;
    }

    private void Settings_OnDeactivated(object? sender, EventArgs e)
    {
        Topmost = true;
        Settings_OnActivated(sender, e);
    }

    private void SettingsMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void NumInput_KeyUp(object sender, KeyEventArgs e)
    {
        var textBox = (TextBox)sender;
        var success = int.TryParse(textBox.Text, out var num);
        if (success)
        {
            textBox.Text = e.Key switch
            {
                Key.Down => (num - 1).ToString(),
                Key.Up => (num + 1).ToString(),
                _ => textBox.Text
            };
        }
    }

    private void MaxWindowNum_KeyDown(object sender, KeyEventArgs e)
    {
        var success = int.TryParse(MaxWindowNum.Text, out var num);
        if (success)
        {
            MaxWindowNum.Text = e.Key switch
            {
                Key.Down => (num - 1).ToString(),
                Key.Up => (num + 1).ToString(),
                _ => MaxWindowNum.Text
            };
        }
    }
}