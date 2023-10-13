using System;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;

namespace SLA_Migration_Try02.Views;

public partial class MainWindow : Window
{
    private const int _restartDuration = 10;
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
        new KeyValuePair<bool, string>(true, "Company / Departmental Gathering"),
        new KeyValuePair<bool, string>(true, "Seminar / Training / Presentation / Lecture"),
        new KeyValuePair<bool, string>(true, "Departmental Discussion"),
        new KeyValuePair<bool, string>(true, "Formal Meeting"),
        new KeyValuePair<bool, string>(true, "Informal Meeting"),
        new KeyValuePair<bool, string>(true, "Other")
    };

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Sort ReasonsAll, by Value (alphabetically)
        ReasonsAll.Sort((x, y) => x.Value.CompareTo(y.Value));

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
        else
        {

        }

        // Timer Initialization
        {
            _backgroundTimer = new DispatcherTimer //(DispatcherPriority.MaxValue)
            {
                Interval = TimeSpan.FromSeconds(_restartDuration)
            };
            _backgroundTimer.Tick += (_, __) =>
            {
                Show();
            };
            _backgroundTimer.Start();

            _foregroundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _foregroundTimer.Tick += (_, __) =>
            {
                // Format of Elapsed Time: "Login - HH:mm:ss", using _startTime, on _closedButton.Content
                _exitTime = DateTime.Now.Subtract(_openTime);
                _closeButton.Content = $"Login - {_exitTime:hh\\:mm\\:ss}";
            };
        }

        // Other Initializations
        {

        }

        // Event Handlers
        {
            Opened += (_, __) =>
            {
                _openTime = DateTime.Now;
                _exitTime = TimeSpan.Zero;
                _foregroundTimer.Start();

                // Resetting Fields
                _reasonsBox.SelectedIndex = -1;
                _reasonsDetail.Text = string.Empty;
                _reasonsDetail.IsVisible = false;
                _closeButton.IsEnabled = false;
            };
            _closeButton.Click += (_, __) =>
            {
                _foregroundTimer.Stop();

                // Handle _exitTime here

                Hide();
            };
            //Closed += (_, __) =>
            //{
            //    _backgroundTimer.Stop();
            //};

            // Others
            _reasonsBox.SelectionChanged += (_, __) =>
            {
                if (_reasonsBox.SelectedItem is KeyValuePair<bool, string> kvp)
                {
                    _reasonsDetail.IsVisible = kvp.Key;
                    _closeButton.IsEnabled = !(kvp.Key && string.IsNullOrWhiteSpace(_reasonsDetail.Text));
                }
            };
            _reasonsDetail.TextChanged += (_, __) =>
            {
                if (_reasonsBox.SelectedItem is KeyValuePair<bool, string> kvp)
                {
                    _closeButton.IsEnabled = !string.IsNullOrWhiteSpace(_reasonsDetail.Text);
                }
            };
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
