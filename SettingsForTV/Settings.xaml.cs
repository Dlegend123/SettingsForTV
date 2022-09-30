using System;
using System.Windows;
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
}