using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using static System.Enum;
using static System.Int32;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace SettingsForTV;

/// <summary>
///     Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private Settings settings;
    public bool AllowSave = false;
    public JObject Mode;
    Rectangle draggedItem;
    Point itemRelativePosition;
    bool IsDragging;
    public string rowCount => Mode["RowCount"]?.ToString() ?? string.Empty;
    public string MaxWindows => Mode[nameof(MaxWindows)]?.ToString() ?? string.Empty;

    public SettingsWindow()
    {
        settings = new Settings();
        Mode = new JObject();
        settings.SetSettings();
        InitializeComponent();
        SetModeSettings();
        Savable = false;
        IsDragging = false;
        CurrentKey.ItemsSource = GetValues(typeof(Key)).Cast<Key>().Distinct().OrderBy(x => x.ToString());
    }

    public bool Savable
    {
        set => AllowSave = value;
    }

    public void SetModeSettings()
    {
        RowCountField.Text =Mode["RowCount"]?.ToString();
        MaxWindowNum.Text = Mode[nameof(MaxWindows)]?.ToString();
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
        var index = settings.Modes.FindIndex(x => x.Value<string>("Name") == ModeNames.Text);
        if (index != -1)
        {
            var mode = settings.Modes[index];
            mode["Name"] = CurrentModeNameField.Text;
            mode[nameof(MaxWindows)] = MaxWindowNum.Text;
            mode["Key"] = CurrentKey.Text;
            settings.Modes[index] = mode;
        }
        else
        {
            settings.Modes.Add(new JObject
            {
                new JProperty("Name", CurrentModeNameField.Text),
                new JProperty("Key", CurrentKey.Text),
                new JProperty(nameof(MaxWindows), MaxWindowNum.Text)
            });
        }
        settings.Save();
        Savable = false;
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
        return MaxWindowNum.Text != "" && RowCountField.Text != "" &&
               (MaxWindowNum.Text != Mode[nameof(MaxWindows)]?.ToString() ||
                RowCountField.Text != Mode["RowCount"]?.ToString());
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

    private void CurrentKey_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _ = TryParse(CurrentKey.SelectedItem.ToString(), out Key key);

        switch (key)
        {
            case Key.None:
                break;
            case Key.Cancel:
                break;
            case Key.Back:
                break;
            case Key.Tab:
                break;
            case Key.LineFeed:
                break;
            case Key.Clear:
                break;
            case Key.Enter:
                break;
            case Key.Pause:
                break;
            case Key.Capital:
                break;
            case Key.HangulMode:
                break;
            case Key.JunjaMode:
                break;
            case Key.FinalMode:
                break;
            case Key.HanjaMode:
                break;
            case Key.Escape:
                break;
            case Key.ImeConvert:
                break;
            case Key.ImeNonConvert:
                break;
            case Key.ImeAccept:
                break;
            case Key.ImeModeChange:
                break;
            case Key.Space:
                break;
            case Key.PageUp:
                break;
            case Key.Next:
                break;
            case Key.End:
                break;
            case Key.Home:
                break;
            case Key.Left:
                break;
            case Key.Up:
                break;
            case Key.Right:
                break;
            case Key.Down:
                break;
            case Key.Select:
                break;
            case Key.Print:
                break;
            case Key.Execute:
                break;
            case Key.PrintScreen:
                break;
            case Key.Insert:
                break;
            case Key.Delete:
                break;
            case Key.Help:
                break;
            case Key.D0:
                break;
            case Key.D1:
                break;
            case Key.D2:
                break;
            case Key.D3:
                break;
            case Key.D4:
                break;
            case Key.D5:
                break;
            case Key.D6:
                break;
            case Key.D7:
                break;
            case Key.D8:
                break;
            case Key.D9:
                break;
            case Key.A:
                break;
            case Key.B:
                break;
            case Key.C:
                break;
            case Key.D:
                break;
            case Key.E:
                break;
            case Key.F:
                break;
            case Key.G:
                break;
            case Key.H:
                break;
            case Key.I:
                break;
            case Key.J:
                break;
            case Key.K:
                break;
            case Key.L:
                break;
            case Key.M:
                break;
            case Key.N:
                break;
            case Key.O:
                break;
            case Key.P:
                break;
            case Key.Q:
                break;
            case Key.R:
                break;
            case Key.S:
                break;
            case Key.T:
                break;
            case Key.U:
                break;
            case Key.V:
                break;
            case Key.W:
                break;
            case Key.X:
                break;
            case Key.Y:
                break;
            case Key.Z:
                break;
            case Key.LWin:
                break;
            case Key.RWin:
                break;
            case Key.Apps:
                break;
            case Key.Sleep:
                break;
            case Key.NumPad0:
                break;
            case Key.NumPad1:
                break;
            case Key.NumPad2:
                break;
            case Key.NumPad3:
                break;
            case Key.NumPad4:
                break;
            case Key.NumPad5:
                break;
            case Key.NumPad6:
                break;
            case Key.NumPad7:
                break;
            case Key.NumPad8:
                break;
            case Key.NumPad9:
                break;
            case Key.Multiply:
                break;
            case Key.Add:
                break;
            case Key.Separator:
                break;
            case Key.Subtract:
                break;
            case Key.Decimal:
                break;
            case Key.Divide:
                break;
            case Key.F1:
                break;
            case Key.F2:
                break;
            case Key.F3:
                break;
            case Key.F4:
                break;
            case Key.F5:
                break;
            case Key.F6:
                break;
            case Key.F7:
                break;
            case Key.F8:
                break;
            case Key.F9:
                break;
            case Key.F10:
                break;
            case Key.F11:
                break;
            case Key.F12:
                break;
            case Key.F13:
                break;
            case Key.F14:
                break;
            case Key.F15:
                break;
            case Key.F16:
                break;
            case Key.F17:
                break;
            case Key.F18:
                break;
            case Key.F19:
                break;
            case Key.F20:
                break;
            case Key.F21:
                break;
            case Key.F22:
                break;
            case Key.F23:
                break;
            case Key.F24:
                break;
            case Key.NumLock:
                break;
            case Key.Scroll:
                break;
            case Key.LeftShift:
                break;
            case Key.RightShift:
                break;
            case Key.LeftCtrl:
                break;
            case Key.RightCtrl:
                break;
            case Key.LeftAlt:
                break;
            case Key.RightAlt:
                break;
            case Key.BrowserBack:
                break;
            case Key.BrowserForward:
                break;
            case Key.BrowserRefresh:
                break;
            case Key.BrowserStop:
                break;
            case Key.BrowserSearch:
                break;
            case Key.BrowserFavorites:
                break;
            case Key.BrowserHome:
                break;
            case Key.VolumeMute:
                break;
            case Key.VolumeDown:
                break;
            case Key.VolumeUp:
                break;
            case Key.MediaNextTrack:
                break;
            case Key.MediaPreviousTrack:
                break;
            case Key.MediaStop:
                break;
            case Key.MediaPlayPause:
                break;
            case Key.LaunchMail:
                break;
            case Key.SelectMedia:
                break;
            case Key.LaunchApplication1:
                break;
            case Key.LaunchApplication2:
                break;
            case Key.Oem1:
                break;
            case Key.OemPlus:
                break;
            case Key.OemComma:
                break;
            case Key.OemMinus:
                break;
            case Key.OemPeriod:
                break;
            case Key.Oem2:
                break;
            case Key.Oem3:
                break;
            case Key.AbntC1:
                break;
            case Key.AbntC2:
                break;
            case Key.Oem4:
                break;
            case Key.Oem5:
                break;
            case Key.Oem6:
                break;
            case Key.Oem7:
                break;
            case Key.Oem8:
                break;
            case Key.Oem102:
                break;
            case Key.ImeProcessed:
                break;
            case Key.System:
                break;
            case Key.DbeAlphanumeric:
                break;
            case Key.DbeKatakana:
                break;
            case Key.DbeHiragana:
                break;
            case Key.DbeSbcsChar:
                break;
            case Key.DbeDbcsChar:
                break;
            case Key.DbeRoman:
                break;
            case Key.Attn:
                break;
            case Key.CrSel:
                break;
            case Key.DbeEnterImeConfigureMode:
                break;
            case Key.DbeFlushString:
                break;
            case Key.DbeCodeInput:
                break;
            case Key.DbeNoCodeInput:
                break;
            case Key.DbeDetermineString:
                break;
            case Key.DbeEnterDialogConversionMode:
                break;
            case Key.OemClear:
                break;
            case Key.DeadCharProcessed:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        IsDragging = true;
        draggedItem = (Rectangle)sender;
        itemRelativePosition = e.GetPosition(draggedItem);
    }

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!IsDragging)
            return;
        IsDragging = false;
    }

    private void UIElement_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!IsDragging)
            return;

        Point canvasRelativePosition = e.GetPosition(MyCanvas);

        Canvas.SetTop(draggedItem, canvasRelativePosition.Y - itemRelativePosition.Y);
        Canvas.SetLeft(draggedItem, canvasRelativePosition.X - itemRelativePosition.X);
    }
}