using Avalonia;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace SLA_Remake;

public partial class MainWindow : Window
{
	private readonly bool IsDesigning = Design.IsDesignMode;
	private const double _reasonsBoxHeight = 35.00 / 100.00;
	private readonly TimeSpan _idleAllowed = TimeSpan.FromMinutes(5);
	private const string _loginPlaceholder = "Login - 00:00:00";

	private readonly TextBox _reasonMore;
	private readonly ListBox _reasonsBox;
	private readonly Button _loginButton;
	private readonly Button _debugButton;

	private static readonly DispatcherTimer _backgroundTimer = new(DispatcherPriority.MaxValue)
	{
		Interval = TimeSpan.FromSeconds(1)
	};
	private static readonly DispatcherTimer _primeGuardTimer = new(DispatcherPriority.MaxValue)
	{
		Interval = TimeSpan.FromMilliseconds(1.5)
	};
	private static DateTime _openTime = DateTime.MinValue;
	private static TimeSpan _exitTime = TimeSpan.Zero;

	public static List<Models.Reason> GetReasons => Reasons.ReasonsAll;

	public MainWindow()
	{
		InitializeComponent();

		// Elemental-Bindings
		{
			_loginButton = this.FindControl<Button>("CloseButton")!;
			_debugButton = this.FindControl<Button>("DebugButton")!;
			_reasonsBox = this.FindControl<ListBox>("ReasonsBox")!;
			_reasonMore = this.FindControl<TextBox>("ReasonDetail")!;
		}

		OtherInitializations();
		BindEvents();
	}

	private void OtherInitializations()
	{
		Reasons.SortReasons();

		// Timer Initialization
		{
			_backgroundTimer.Tick += BackgroundTimer_Tick!;
			_primeGuardTimer.Tick += !Configuration.EnablePrimeGuard || IsDesigning ? NoOperation! : PrimeGuard_Tick!;
			_backgroundTimer.Start();
		}

		// Overriding Accent Color
		{
			Application.Current!.Resources["SystemAccentColor"] = Color.Parse("#F44336");
		}

		if (IsDesigning) return;

		// Registering for Startup
		{
			CrossUtility.RegisterForStartup();
		}

		// Starting Jobs
		{
			Jobs.Launch();
		}

		// Restricting Keys
		{
			var handleEvent = (object sender, KeyEventArgs e) => e.Handled = Restricted.Keys.Contains(e.Key) || Restricted.Modifiers.Contains(e.KeyModifiers);
			AddHandler(KeyDownEvent, handleEvent, handledEventsToo: true);
		}

		// Pin to all Desktops (Windows)
		{
			var handle = TryGetPlatformHandle()!;
			CrossUtility.PinToAllDesktops(handle.Handle);
		}
	}

	// Helper Methods:
	// ---------------

	private void BindEvents()
	{
		// Main-Window specific
		Opened += WindowOpened!;
		Closed += WindowClosed!;
		Closing += WindowClosing!;

		// Others
		_loginButton.Click += LoginButton_Click!;
		_reasonsBox.SelectionChanged += ReasonsBox_SelectionChanged!;
		_reasonMore.TextChanged += ReasonsDetail_TextChanged!;
	}

	private void ResetFields()
	{
		_reasonsBox.SelectedIndex = -1;
		_reasonMore.Text = string.Empty;
		_reasonMore.IsVisible = false;
		_loginButton.IsEnabled = false;
	}

	private void ForceActivate(object? _ = null, object? __ = null)
	{
		Activate();
		this.BringIntoView();
	}

	private static void NoOperation(object _, object __) { }

	// Event Handlers:
	// ---------------

	private void WindowOpened(object _, EventArgs __)
	{
		// Time Tracking
		// -------------

		_openTime = DateTime.Now;
		_exitTime = TimeSpan.Zero;
		_loginButton.Content = _loginPlaceholder;

		// Timers Initialization
		// ---------------------

		_backgroundTimer.Tick += ForegroundTimer_TickTask!;
		Deactivated += Configuration.EnablePrimeGuard ? ForceActivate! : NoOperation!;
		_primeGuardTimer.Start();

		// Height Adjustment
		// -----------------

		_reasonsBox.MaxHeight = CrossUtility.Screen.High * _reasonsBoxHeight;

		if (IsDesigning) return;

		// Database Logging
		// ----------------

		var entry = Models.LogEntry.CreateLogout();
		Database<Models.LogEntry>.Save(entry);
	}

	public static void WindowClosing(object _, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	public static void WindowClosed(object _, EventArgs __)
	{
		// Reinitiating measures can be Taken here
	}

	private void PrimeGuard_Tick(object _, object __)
	{
		ForceActivate();
		CrossUtility.RestrictOSFeatures();
	}

	private void BackgroundTimer_Tick(object _, EventArgs __)
	{
		if (CrossUtility.GetIdleTime() < _idleAllowed) return;
		Show();
		this.BringIntoView();
	}

	private void ForegroundTimer_TickTask(object _, EventArgs __)
	{
		_exitTime = DateTime.Now.Subtract(_openTime);
		_loginButton.Content = $@"Login - {_exitTime:hh\:mm\:ss}";
		_debugButton.Content = $@"Debug - {CrossUtility.GetIdleTime():hh\:mm\:ss}";
	}

	private void LoginButton_Click(object _, RoutedEventArgs __)
	{
		// Stopping Timers
		// ---------------

		_backgroundTimer.Tick -= ForegroundTimer_TickTask!;
		Deactivated -= ForceActivate!;
		_primeGuardTimer.Stop();

		// _exitTime Interval can be handled here

		if (IsDesigning) return;

		// Database Logging
		// ----------------

		var entry = Models.LogEntry.CreateLogin(
			reasonDetail: _reasonMore.Text ?? Configuration.NotAvailableOrFound,
			reason: (Models.Reason)_reasonsBox.SelectedItem!
		);
		Database<Models.LogEntry>.Save(entry);

		// Release Restrictions
		// --------------------

		Hide();
		CrossUtility.AllowOSFeatures();
		ResetFields();
	}

	private void ReasonsBox_SelectionChanged(object _, SelectionChangedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not Models.Reason kvp) return;
		_reasonMore.IsVisible = kvp.RequiresMoreDetail;
		_loginButton.IsEnabled = !(kvp.RequiresMoreDetail && string.IsNullOrWhiteSpace(_reasonMore.Text));
	}

	private void ReasonsDetail_TextChanged(object _, RoutedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not Models.Reason) return;
		_loginButton.IsEnabled = !string.IsNullOrWhiteSpace(_reasonMore.Text) && _reasonMore.Text.Length > 4;
	}

	private void ReasonsDetail_Entered(object sender, KeyEventArgs e)
	{
		if (e.Key != Key.Enter) return;
		LoginButton_Click(sender, e);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
		DataContext = this;
	}
}
