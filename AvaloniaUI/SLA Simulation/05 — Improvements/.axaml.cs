using System;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;

namespace SLA_Migration_Try02.Views;

public partial class MainWindow : Window
{
    private const int _restartDuration = 10;
    private const string _loginPlaceholder = "Login - 00:00:00";

    private readonly Border _newComerGuide;
    private readonly TextBox _reasonsDetail;
    private readonly Button _closeButton;
    private readonly ListBox _reasonsBox;
    private readonly DispatcherTimer _backgroundTimer;
    private readonly DispatcherTimer _foregroundTimer;
    private static DateTime _openTime = DateTime.MinValue;
    private static TimeSpan _exitTime = TimeSpan.Zero;

    public static List<KeyValuePair<bool, string>> ReasonsAll { get; set; } = new()
    {
        new KeyValuePair<bool, string>(false, "System Reboot"),
        new KeyValuePair<bool, string>(false, "Break"),
        new KeyValuePair<bool, string>(false, "Rest Room"),
        new KeyValuePair<bool, string>(false, "Namaz"),
        new KeyValuePair<bool, string>(false, "Lunch"),
        new KeyValuePair<bool, string>(false, "Dinner"),
        new KeyValuePair<bool, string>(false, "Late Leave"),
        new KeyValuePair<bool, string>(false, "Short Leave"),
        new KeyValuePair<bool, string>(false, "Half Leave"),
        new KeyValuePair<bool, string>(true, "On Official Call / Duty"),
        new KeyValuePair<bool, string>(true, "Filing / Paper Work"),
        new KeyValuePair<bool, string>(true, "Official Outgoing"),
        new KeyValuePair<bool, string>(true, "Official Gathering"),
        new KeyValuePair<bool, string>(true, "Seminar / Training / Presentation / Lecture"),
        new KeyValuePair<bool, string>(true, "Departmental Discussion"),
        new KeyValuePair<bool, string>(true, "Formal Meeting"),
        new KeyValuePair<bool, string>(true, "Informal Meeting"),
        new KeyValuePair<bool, string>(true, "Other")
    };

    public static readonly List<Key> RestrictedKeys = new()
    {
        Key.LeftAlt,    Key.RightAlt,
        Key.LWin,       Key.RWin,
        Key.F1,         Key.F2,         Key.F3,         Key.F4,         Key.F5,
        Key.F6,         Key.F7,         Key.F8,         Key.F9,         Key.F10,
        Key.F11,        Key.F12,        Key.F13,        Key.F14,        Key.F15,
        Key.F16,        Key.F17,        Key.F18,        Key.F19,        Key.F20,
        Key.F21,        Key.F22,        Key.F23,        Key.F24,
    };

    public MainWindow()
    {
        InitializeComponent();

        // Elemental-Bindings
        {
            _newComerGuide = this.FindControl<Border>("NewComerGuide")!;
            _closeButton = this.FindControl<Button>("CloseButton")!;
            _reasonsBox = this.FindControl<ListBox>("ReasonsBox")!;
            _reasonsDetail = this.FindControl<TextBox>("txtReasonDetail")!;
        }

        if (OperatingSystem.IsMacOS())
        {

        }
        else if (OperatingSystem.IsWindows())
        {

        }
        else
        {

        }

        // Timer Initialization
        {
            _backgroundTimer = new DispatcherTimer //(DispatcherPriority.MaxValue)
            {
                Interval = TimeSpan.FromSeconds(_restartDuration)
            };
            _backgroundTimer.Tick += BackgroundTimer_Tick;
            _backgroundTimer.Start();

            _foregroundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _foregroundTimer.Tick += ForegroundTimer_Tick;
        }

        OtherInitializations();
        BindEvents();
    }

    private void OtherInitializations()
    {
        ReasonsAll.Sort((x, y) => x.Value.CompareTo(y.Value));

        var x = (object sender, KeyEventArgs e) =>
        {
            // Suppress certain keys within the application
            if (RestrictedKeys.Contains(e.Key))
            {
                e.Handled = true;
            }
        };

        AddHandler(KeyDownEvent, x, handledEventsToo: true);
    }

    private void BindEvents()
    {
        // Window specific
        Opened += WindowOpened;
        Closed += WindowClosed;

        // Others
        _closeButton.Click += LoginButton_Click;
        _reasonsBox.SelectionChanged += ReasonsBox_SelectionChanged;
        _reasonsDetail.TextChanged += ReasonsDetail_TextChanged;
    }


    private void WindowOpened(object? _, EventArgs __)
    {
        _openTime = DateTime.Now;
        _exitTime = TimeSpan.Zero;
        _foregroundTimer.Start();
        _closeButton.Content = _loginPlaceholder;
    }

    private void WindowClosed(object? _, EventArgs __)
    {
        // Disable Window Closing
    }

    private void ForegroundTimer_Tick(object? _, EventArgs __)
    {
        _exitTime = DateTime.Now.Subtract(_openTime);
        _closeButton.Content = $"Login - {_exitTime:hh\\:mm\\:ss}";
    }

    private void BackgroundTimer_Tick(object? _, EventArgs __)
    {
        Show();
    }

    private void LoginButton_Click(object? _, RoutedEventArgs __)
    {
        _foregroundTimer.Stop();

        // Handle _exitTime here

        // Resetting Fields
        _reasonsBox.SelectedIndex = -1;
        _reasonsDetail.Text = string.Empty;
        _reasonsDetail.IsVisible = false;
        _closeButton.IsEnabled = false;

        Hide();
    }

    private void ReasonsBox_SelectionChanged(object? _, SelectionChangedEventArgs __)
    {
        if (_reasonsBox.SelectedItem is KeyValuePair<bool, string> kvp)
        {
            _reasonsDetail.IsVisible = kvp.Key;
            _closeButton.IsEnabled = !(kvp.Key && string.IsNullOrWhiteSpace(_reasonsDetail.Text));
        }
    }

    private void ReasonsDetail_TextChanged(object? _, RoutedEventArgs __)
    {
        if (_reasonsBox.SelectedItem is KeyValuePair<bool, string>)
        {
            _closeButton.IsEnabled = !string.IsNullOrWhiteSpace(_reasonsDetail.Text) && _reasonsDetail.Text.Length > 4;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = this;
    }
}
