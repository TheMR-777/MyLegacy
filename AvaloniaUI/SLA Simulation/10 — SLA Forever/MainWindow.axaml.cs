using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SLA_Remake;

public partial class MainWindow : Window
{
	private const int _idleDuration = 10;
	private const string _loginPlaceholder = "Login - 00:00:00";

	private readonly TextBox _reasonsDetail;
	private readonly ListBox _reasonsBox;
	private readonly Button _closeButton;
	private readonly Button _debugButton;

	private static readonly DispatcherTimer _backgroundTimer = new(DispatcherPriority.MaxValue)
    {
        Interval = TimeSpan.FromSeconds(1)
    };
	private static DateTime _openTime = DateTime.MinValue;
	private static TimeSpan _exitTime = TimeSpan.Zero;

	public static List<KeyValuePair<bool, string>> ReasonsAll { get; } = new()
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
		// Alt, Ctrl, etc. are handled separately
		Key.Escape,     Key.Insert,		Key.Home,		Key.End,		Key.Delete,
		Key.F1,         Key.F2,         Key.F3,         Key.F4,         Key.F5,
		Key.F6,         Key.F7,         Key.F8,         Key.F9,         Key.F10,
		Key.F11,        Key.F12,        Key.F13,        Key.F14,        Key.F15,
		Key.F16,        Key.F17,        Key.F18,        Key.F19,        Key.F20,
		Key.F21,        Key.F22,        Key.F23,        Key.F24,

        Key.PageUp,		Key.PageDown,					Key.MediaNextTrack,
		Key.KanaMode,	Key.KanjiMode,					Key.MediaPreviousTrack,
        Key.VolumeUp,	Key.VolumeDown,					Key.VolumeMute,
		Key.MediaStop,	Key.MediaPlayPause,				Key.SelectMedia,
		Key.LaunchMail,	Key.LaunchApplication1,			Key.LaunchApplication2
	};

	public static readonly List<KeyModifiers> RestrictedModifiers = new()
	{
        KeyModifiers.Alt,		KeyModifiers.Control,		KeyModifiers.Meta,
    };

	public MainWindow()
	{
		InitializeComponent();

		// Elemental-Bindings
		{
			_closeButton = this.FindControl<Button>("CloseButton")!;
			_debugButton = this.FindControl<Button>("DebugButton")!;
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

		OtherInitializations();
		BindEvents();
	}

	private void OtherInitializations()
	{
		Utility.DisableGestures();
        ReasonsAll.Sort((x, y) => x.Value.CompareTo(y.Value));

        // Timer Initialization
        {
            _backgroundTimer.Tick += BackgroundTimer_Tick;
            _backgroundTimer.Start();
        }

		// Restricting Keys
		{
            var handleEvent = (object sender, KeyEventArgs e) => e.Handled = RestrictedKeys.Contains(e.Key) || RestrictedModifiers.Contains(e.KeyModifiers);
            AddHandler(KeyDownEvent, handleEvent, handledEventsToo: true);
        }
    }

    // Helper Methods:
    // ---------------

    private void BindEvents()
    {
        // Main-Window specific
        Opened += WindowOpened;
        Closed += WindowClosed;
        Closing += WindowClosing;

        // Others
        _closeButton.Click += LoginButton_Click;
        _reasonsBox.SelectionChanged += ReasonsBox_SelectionChanged;
        _reasonsDetail.TextChanged += ReasonsDetail_TextChanged;
    }

    private void ResetFields()
    {
        _reasonsBox.SelectedIndex = -1;
        _reasonsDetail.Text = string.Empty;
        _reasonsDetail.IsVisible = false;
        _closeButton.IsEnabled = false;
    }

    // Event Handlers:
    // ---------------

    private void WindowOpened(object _, EventArgs __)
	{
		_openTime = DateTime.Now;
		_exitTime = TimeSpan.Zero;
		_closeButton.Content = _loginPlaceholder;
        _backgroundTimer.Tick += ForegroundTimer_TickTask;
    }

	public static void WindowClosing(object _, CancelEventArgs e)
	{
        e.Cancel = true;
    }

	public static void WindowClosed(object _, EventArgs __)
	{
		// Disable Window Closing
	}

	private void BackgroundTimer_Tick(object _, EventArgs __)
	{
		if (Utility.GetIdleTime() >= TimeSpan.FromSeconds(_idleDuration))
		{
            Show();
			this.BringIntoView();
        }
	}

	private void ForegroundTimer_TickTask(object _, EventArgs __)
	{
		_exitTime = DateTime.Now.Subtract(_openTime);
		_closeButton.Content = $"Login - {_exitTime:hh\\:mm\\:ss}";
		_debugButton.Content = $"Debug - {Utility.GetIdleTime():hh\\:mm\\:ss}";
    }

	private void LoginButton_Click(object _, RoutedEventArgs __)
	{
		_backgroundTimer.Tick -= ForegroundTimer_TickTask;

		// Handle _exitTime here

		ResetFields();
		Hide();
	}

	private void ReasonsBox_SelectionChanged(object _, SelectionChangedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is KeyValuePair<bool, string> kvp)
		{
			_reasonsDetail.IsVisible = kvp.Key;
			_closeButton.IsEnabled = !(kvp.Key && string.IsNullOrWhiteSpace(_reasonsDetail.Text));
		}
	}

	private void ReasonsDetail_TextChanged(object _, RoutedEventArgs __)
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

public class Utility
{
    public static void LogOut_theUser(object sender, RoutedEventArgs e)
    {
#if WIN
        var cmd = "rundll32.exe";
        var arg = "user32.dll,LockWorkStation";
#elif MAC
		var cmd = "osascript";
		var arg = "-e 'tell application \"System Events\" to keystroke \"q\" using {command down, control down, option down}'";
#endif
        var startInfo = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = arg,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
    }

    public static TimeSpan GetIdleTime()
    {
#if WIN
        var lastInPut = new LowLevel_APIs.LASTINPUTINFO
        {
            cbSize = (uint)Marshal.SizeOf(typeof(LowLevel_APIs.LASTINPUTINFO)),
            dwTime = 0
        };
        LowLevel_APIs.GetLastInputInfo(ref lastInPut);

        return TimeSpan.FromMilliseconds(Environment.TickCount - lastInPut.dwTime);
#elif MAC
		var startInfo = new ProcessStartInfo
		{
			FileName = "/bin/bash",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			Arguments = "-c \"ioreg -c IOHIDSystem | awk '/HIDIdleTime/ {print $NF/1000000000; exit}'\""
		};

		using var process = new Process { StartInfo = startInfo };
		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		var idleTimeSec = double.TryParse(output, out var result) ? result : 0;
		return TimeSpan.FromSeconds(idleTimeSec);
#endif
    }

	public static void DisableGestures()
	{
#if WIN
		var hWnd = Process.GetCurrentProcess().MainWindowHandle;
        IntPtr style = LowLevel_APIs.SetWindowLongPtr(hWnd, LowLevel_APIs.GWL_EXSTYLE, (IntPtr)(LowLevel_APIs.GetWindowLongPtr(hWnd, LowLevel_APIs.GWL_EXSTYLE) & ~LowLevel_APIs.WS_EX_NOREDIRECTIONBITMAP));
#endif
    }
}

public partial class LowLevel_APIs
{
#if WIN
	// Fetching Idle Time
	// ------------------

    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetLastInputInfo(ref LASTINPUTINFO plii);

    // Disabling Gestures
    // ------------------

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

    [LibraryImport("user32.dll")]
    public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

	[LibraryImport("user32.dll")]
	public static partial IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
#endif
}