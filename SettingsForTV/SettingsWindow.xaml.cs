using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using static System.Int32;

namespace SettingsForTV;

/// <summary>
///     Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private Settings settings;
    public bool AllowSave = false;

    public string rowCount => settings.Display["RowCount"]?.ToString() ?? string.Empty;

    public string MaxWindows => settings.Display[nameof(MaxWindows)]?.ToString() ?? string.Empty;

    public SettingsWindow()
    {
        settings = new Settings();
        settings.SetSettings();
        InitializeComponent();
        
        SetDisplaySettings();
        Savable = false;
    }

    public bool allowOverlay => (bool)(settings.Display["AllowOverlap"] ?? false);

    public bool Savable
    {
        set => SaveSettings.IsEnabled = AllowSave = value;
    }

    public void SetDisplaySettings()
    {
        var setting = settings.Display;
        RowCount.Text = setting["RowCount"]?.ToString();
        MaxWindowNum.Text = setting[nameof(MaxWindows)]?.ToString();
        AllowOverlap.IsChecked = (bool)(setting["AllowOverlap"] ?? false);
    }

    public bool Save()
    {
        return AllowSave;
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
        _ = TryParse(textBox.Text, out var num);

        if (e.Key != Key.Down)
        {
            if (e.Key == Key.Up)
            {
                textBox.Text = (num + 1).ToString();
            }
        }
        else
        {
            if (num - 1 >= 1)
            {
                textBox.Text = (num - 1).ToString();
            }
        }
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        settings.Display = new JObject()
        {
            new JProperty("RowCount", RowCount.Text),
            new JProperty("MaxWindows", MaxWindowNum.Text),
            new JProperty("AllowOverlap", AllowOverlap.IsChecked)
        };
        settings.Save();
        settings=Settings.GetSettings();
    }

    private void RowCount_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckToAllowSave();

    }

    public void CheckToAllowSave()
    {
        Savable = SaveDisplaySettings();
    }

    private bool SaveDisplaySettings()
    {
        return MaxWindowNum.Text != "" && RowCount.Text != "" &&
               (MaxWindowNum.Text != settings.Display[nameof(MaxWindows)]?.ToString() ||
                AllowOverlap.IsChecked != (bool)(settings.Display["AllowOverlap"] ?? false) ||
                RowCount.Text != settings.Display["RowCount"]?.ToString());
    }

    private void MaxWindowNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        CheckToAllowSave();
    }

    private void AllowOverlap_Checked(object sender, RoutedEventArgs e)
    {
        CheckToAllowSave();
    }

    private void AllowOverlap_Unchecked(object sender, RoutedEventArgs e)
    {
        CheckToAllowSave();
    }
}