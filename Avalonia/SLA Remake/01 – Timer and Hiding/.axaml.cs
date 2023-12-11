using System;
using System.Reactive;
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
    private readonly Button _closeButton;
    private readonly DispatcherTimer _backgroundTimer;

    public MainWindow()
    {
        InitializeComponent();
        _closeButton = this.FindControl<Button>("CloseButton")!;

        // Timer Initialization
        {
            _backgroundTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(_restartDuration)
            };
            _backgroundTimer.Tick += (s, e) =>
            {
                Show();
            };
        }

        // Event Handlers
        {
            Opened += (_, __) =>
            {
                _backgroundTimer.Start();
            };
            _closeButton.Click += (_, __) =>
            {
                Hide();
            };
            Closed += (_, __) =>
            {
                _backgroundTimer.Stop();
            };
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
