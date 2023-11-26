using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media;
using System.Net.Http;
using Dapper;

namespace SLA_Remake;

public static class Constants
{
	// Command and Control
	// -------------------

	public const bool EnableWebAPI = true;
	public const bool EnablePrimeGuard = true;
	public const bool EnableDiscordReporting = true;
	public const bool EnableDatabaseLogging = true;

	// Other Constants
	// ---------------

	public const string ApplicationVersion = "3.0.0.00";
	public const string DiscordWebhookURL = "https://discord.com/api/webhooks/1172483585698185226/M1oWUKwwl-snXr6sHTeAQoKYQxmg4JVg-tRKkqUZ1gSuYXwsV5Q9DhZj00mMX0_iui6d";
}

public partial class MainWindow : Window
{
	private readonly bool IsDesigning = Design.IsDesignMode;
	private readonly TimeSpan _idleAllowed = TimeSpan.FromSeconds(10);
	private const string _loginPlaceholder = "Login - 00:00:00";

	private readonly TextBox _reasonsDetail;
	private readonly ListBox _reasonsBox;
	private readonly Button _closeButton;
	private readonly Button _debugButton;

	private static readonly DispatcherTimer _backgroundTimer = new(DispatcherPriority.MaxValue)
	{
		Interval = TimeSpan.FromSeconds(1)
	};
	private static readonly DispatcherTimer _primeGuardTimer = new(DispatcherPriority.MaxValue)
	{
		Interval = TimeSpan.FromMilliseconds(1)
	};
	private static readonly DispatcherTimer _noInternetTimer = new(DispatcherPriority.MaxValue)
	{
		Interval = TimeSpan.FromSeconds(10)
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
		Key.Escape,     Key.Insert,     Key.Home,       Key.End,        Key.Delete,
		Key.F1,         Key.F2,         Key.F3,         Key.F4,         Key.F5,
		Key.F6,         Key.F7,         Key.F8,         Key.F9,         Key.F10,
		Key.F11,        Key.F12,        Key.F13,        Key.F14,        Key.F15,
		Key.F16,        Key.F17,        Key.F18,        Key.F19,        Key.F20,
		Key.F21,        Key.F22,        Key.F23,        Key.F24,

		Key.PageUp,     Key.PageDown,                   Key.MediaNextTrack,
		Key.KanaMode,   Key.KanjiMode,                  Key.MediaPreviousTrack,
		Key.VolumeUp,   Key.VolumeDown,                 Key.VolumeMute,
		Key.MediaStop,  Key.MediaPlayPause,             Key.SelectMedia,
		Key.LaunchMail, Key.LaunchApplication1,         Key.LaunchApplication2
	};

	public static readonly List<KeyModifiers> RestrictedModifiers = new()
	{
		KeyModifiers.Alt,       KeyModifiers.Control,       KeyModifiers.Meta,
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

		if (OperatingSystem.IsWindows())
		{

		}
		else if (OperatingSystem.IsMacOS())
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
		ReasonsAll.Sort((x, y) => string.Compare(x.Value, y.Value, StringComparison.OrdinalIgnoreCase));

		// Timer Initialization
		{
			_backgroundTimer.Tick += BackgroundTimer_Tick;
			_primeGuardTimer.Tick += !Constants.EnablePrimeGuard || IsDesigning ? ForceActivate : PrimeGuard_Tick;
			_noInternetTimer.Tick += Internet_TickTask;
			_backgroundTimer.Start();
		}

		// Overriding Accent Color
		{
			Application.Current!.Resources["SystemAccentColor"] = Color.Parse("#F44336");
		}

		if (IsDesigning) return;

		// Restricting Keys
		{
			var handleEvent = (object sender, KeyEventArgs e) => e.Handled = RestrictedKeys.Contains(e.Key) || RestrictedModifiers.Contains(e.KeyModifiers);
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

	private void ForceActivate(object _ = null, object __ = null)
	{
		Activate();
		this.BringIntoView();
	}

	private static void RunNoInternetJob()
	{
		if (_noInternetTimer.IsEnabled) return;
		_noInternetTimer.Start();
	}

	// Event Handlers:
	// ---------------

	private void WindowOpened(object _, EventArgs __)
	{
		_openTime = DateTime.Now;
		_exitTime = TimeSpan.Zero;
		_closeButton.Content = _loginPlaceholder;

		_backgroundTimer.Tick += ForegroundTimer_TickTask;
		Deactivated += ForceActivate;
		_primeGuardTimer.Start();

		if (!WebAPI.VerifyDatabase())
		{
			// 1. Save the entry to the Database
			RunNoInternetJob();
			return;
		}
		// Post the entries to the API
	}

	public static void WindowClosing(object _, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	public static void WindowClosed(object _, EventArgs __)
	{
		// Disable Window Closing
	}

	private void PrimeGuard_Tick(object _ = null, object __ = null)
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
		_closeButton.Content = $@"Login - {_exitTime:hh\:mm\:ss}";
		_debugButton.Content = $@"Debug - {CrossUtility.GetIdleTime():hh\:mm\:ss}";
	}

	private static void Internet_TickTask(object _ = null, object __ = null)
	{
		if (!WebAPI.VerifyDatabase()) return;

		var entries = Database.GetSavedEntries();
		// post the entries to the API
		// if (could not be posted) return;
		Database.Clear();
		_noInternetTimer.Stop();
	}

	private void LoginButton_Click(object _, RoutedEventArgs __)
	{
		_backgroundTimer.Tick -= ForegroundTimer_TickTask;
		Deactivated -= ForceActivate;
		_primeGuardTimer.Stop();

		// Handle _exitTime here

		CrossUtility.AllowOSFeatures();
		ResetFields();
		Hide();

		if (!WebAPI.VerifyDatabase())
		{
			// 1. Update the entry to the Database
			RunNoInternetJob();
			return;
		}
		// Post the entries to the API
	}

	private void ReasonsBox_SelectionChanged(object _, SelectionChangedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not KeyValuePair<bool, string> kvp) return;
		_reasonsDetail.IsVisible = kvp.Key;
		_closeButton.IsEnabled = !(kvp.Key && string.IsNullOrWhiteSpace(_reasonsDetail.Text));
	}

	private void ReasonsDetail_TextChanged(object _, RoutedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not KeyValuePair<bool, string>) return;
		_closeButton.IsEnabled = !string.IsNullOrWhiteSpace(_reasonsDetail.Text) && _reasonsDetail.Text.Length > 4;
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
		DataContext = this;
	}
}

public static class WebAPI
{
	private static readonly HttpClient _webClient = new()
	{
		BaseAddress = new Uri("https://sla.cash-n-cash.co.uk/api/products/")
	};

	// The Endpoints
	// -------------

	private const string deployedVersion = "GetCurrentSLAVersion";
	private const string databaseConnect = "GetCheckDBConnection";
	private const string logEntryLogging = "GetSLALogEntries";

	// Main Methods
	// ------------

	public static bool VerifyVersion()
	{
		if (!ConnectedToInternet()) return true;
		var version = GetDataFrom(deployedVersion);

		return new Version(Constants.ApplicationVersion).CompareTo(new Version(version)) >= 0;
	}

	public static bool VerifyDatabase()
	{
		if (!ConnectedToInternet()) return false;
		var status = GetDataFrom(databaseConnect);

		try
		{
			return bool.Parse(status);
		}
		catch
		{
			return false;
		}
	}

	public static void L1_RegisterException(Exception x)
	{
		// Fetching Footprints
		// -------------------

		var footprints = new StackTrace().GetFrames()!
			.Select(frame => frame.GetMethod()!)
			.Select(methodReference => $"{methodReference.DeclaringType!}.{methodReference.Name}({string.Join(", ", methodReference.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray())})").ToList();

		// Fetching User Information
		// -------------------------

		var userInf = new[]
		{
			CrossUtility.GetCurrentUser(),
			Environment.MachineName,
		};

		var footers = new[]
		{
			// Application Version
			$"v{Constants.ApplicationVersion}",

			// Timestamp
			$"{DateTime.Now:dd-MM-yyyy hh':'mm':'ss tt}"
		};

		// Body Building
		// -------------

		var body = new
		{
			username = "SLA Remake - Logger",
			avatar_url = "https://i.imgur.com/IvgCM1R.png",
			content = "",
			embeds = new List<object>
			{
				new
				{
					author = new
					{
						name = string.Join("  |  ", userInf),
						url = "https://www.google.com",
						icon_url = "https://i.imgur.com/xCvzudW.png"
					},
					color = 16007990,
					thumbnail = new
					{
						url = "https://i.imgur.com/IvgCM1R.jpg"
					},
					fields = new List<Dictionary<string, string>>
					{
						new()
						{
							{ "name", "Method Trail" },
							{ "value", string.Join(" < ", footprints)},
							{ "inline", "true" }
						},
						new()
						{
							{ "name", "Exception/Message" },
							{ "value", $"```{x.Message}```" },
						},
						new()
						{
							{ "name", "Stack-Trace" },
							{ "value", $"```{x.StackTrace}```" },
						}
					},
					footer = new
					{
						text = string.Join("   |   ", footers),
						icon_url = "https://i.imgur.com/0jqoy6w.jpg"
					}
				}
			}
		};

		// Sending the Request
		// -------------------

		using var request = new HttpRequestMessage(HttpMethod.Post, Constants.DiscordWebhookURL)
		{
			Content = new StringContent(
				System.Text.Json.JsonSerializer.Serialize(body),
				new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
			)
		};
		_ = _webClient.SendAsync(request).Result;
	}

	// Utilities
	// ---------

	public static bool ConnectedToInternet()
	{
		var connected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
		if (!connected) return false;

		// GetIsNetworkAvailable is Fast but Unreliable for 'true'
		// So, we go for the slower method, if that returns 'true'

		try
		{
			using var web = new HttpClient();
			using var res = web.GetAsync("https://google.com/generate_204").Result;
			connected = res.IsSuccessStatusCode;
		}
		catch
		{
			connected = false;
		}
		return connected;
	}

	private static string GetDataFrom(string endPoint)
	{
		using var res = _webClient.GetAsync(endPoint).Result;
		return res.Content.ReadAsStringAsync().Result.Trim('"');
	}
}

public static class Database
{
	public class LogEntry
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string UserIp { get; set; }
		public string LogDate { get; set; }
		public string LogInTime { get; set; }
		public string LogOutTime { get; set; }
		public string LogFlag { get; set; }
		public string Reason { get; set; }
		public string ReasonType { get; set; }
		public string ReasonId { get; set; }
		public string UserPCName { get; set; }
		public string UserDisplayName { get; set; }
		public string LogSide { get; set; }
	}

	public static class QueryBuilder
	{
		private static readonly Type _type_i = typeof(LogEntry);
		private static readonly string _name = _type_i.Name;
		private static readonly System.Reflection.PropertyInfo[] _properties = _type_i.GetProperties();
		private static readonly List<string> _header = _properties.Select(p => p.Name).ToList();
		private static readonly List<string> _values = _properties.Select(p => $"@{p.Name}").ToList();

		public static string GenerateTable()
		{
			var query = $"CREATE TABLE IF NOT EXISTS {_name} ({string.Join(", ", _header)});";
			return query;
		}

		public static string Insert()
		{
			var query = $"INSERT INTO {_name} ({string.Join(", ", _header)}) VALUES ({string.Join(", ", _values)});";
			return query;
		}

		public static string UpdateLast()
		{
			var query = $"UPDATE {_name} SET {string.Join(", ", _header.Select(h => $"{h} = @{h}"))} WHERE ROWID = (SELECT MAX(ROWID) FROM {_name});";
			return query;
		}

		public static string SelectAll()
		{
			var query = $"SELECT * FROM {_name};";
			return query;
		}

		public static string DropTable()
		{
			var query = $"DROP TABLE IF EXISTS {_name};";
			return query;
		}
	}

	private const string SQLiteStorageLoc = "Database.sqlite";
	private const string ConnectionString = $"Data Source={SQLiteStorageLoc};Version=3;";
	private static readonly System.Data.SQLite.SQLiteConnection Connection = new(ConnectionString);

	public static int Save(LogEntry entry = null)
	{
		Connection.Open();
		var query = QueryBuilder.GenerateTable();
		var result = Connection.Execute(query);

		if (entry != null)
		{
			query = QueryBuilder.Insert();
			result = Connection.Execute(query, entry);
		}

		Connection.Close();
		return result;
	}

	public static List<LogEntry> GetSavedEntries()
	{
		Connection.Open();
		var query = QueryBuilder.SelectAll();
		var result = Connection.Query<LogEntry>(query).ToList();
		Connection.Close();
		return result;
	}

	public static int Update(LogEntry entry)
	{
		Connection.Open();
		var query = QueryBuilder.UpdateLast();
		var result = Connection.Execute(query, entry);
		Connection.Close();
		return result;
	}

	public static int Clear()
	{
		Connection.Open();
		var query = QueryBuilder.DropTable();
		var result = Connection.Execute(query);
		Connection.Close();
		return result;
	}
}

public static partial class CrossUtility
{
	public static partial class Screen
	{
#if WIN
		[LibraryImport("user32.dll")]
		private static partial int GetSystemMetrics(int nIndex);

		public static int Width => GetSystemMetrics(0);
		public static int Height => GetSystemMetrics(1);
#elif MAC
		public static readonly int Width = LowLevel_APIs.CoreGraphics.CGDisplayPixelsWide(LowLevel_APIs.CoreGraphics.CGMainDisplayID());
		public static readonly int Height = LowLevel_APIs.CoreGraphics.CGDisplayPixelsHigh(LowLevel_APIs.CoreGraphics.CGMainDisplayID());
#endif
	}

	public static string GetCurrentUser()
	{
#if WIN
		// Note for the Complication in-case on Windows
		// --------------------------------------------
		// Here, Username is fetched of the currently Logged-In User
		// The Environment.UserName is not reliable in this case
		// as it returns the Username of the User who started the App.
		// So, Username is fetched, from the currently active Session.

		var sessionId = LowLevel_APIs.WTSGetActiveConsoleSessionId();
		var username = "N/A";

		if (sessionId == 0xFFFFFFFF)
			return username;

		if (!LowLevel_APIs.WTSQuerySessionInformation(IntPtr.Zero, sessionId, LowLevel_APIs.WTS_INFO_CLASS.WTSUserName,	out var buffer, out _)) 
			return username;

		username = Marshal.PtrToStringAnsi(buffer) ?? username;
		LowLevel_APIs.WTSFreeMemory(buffer);
		return username;
#elif MAC
		return Environment.UserName;
#endif
	}

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

		Process.Start(startInfo);
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

	public static void PinToAllDesktops(IntPtr hwnd)
	{
#if WIN
		// WS_EX_TOOLWINDOW - 3 in 1 Solution
		// ----------------------------------
		// Setting the App as ToolWindow, makes the Windows thinks of it as a Popup.
		// Thus it is not shown in the Taskbar, nor in the Alt+Tab, and Win+Tab list.
		// As a good side-effect we also get the App to be shown on all Virtual-Desktops.

		var style = LowLevel_APIs.GetWindowLongPtr(hwnd, LowLevel_APIs.GWL_EXSTYLE);
		LowLevel_APIs.SetWindowLongPtr(new HandleRef(null, hwnd), LowLevel_APIs.GWL_EXSTYLE, style.ToInt32() | LowLevel_APIs.WS_EX_TOOLWINDOW);
#endif
	}

	public static void RestrictOSFeatures()
	{
		ConstrainCursor();

#if WIN
#elif MAC
		LowLevel_APIs.HideMenuBar();
#endif
	}

	public static void AllowOSFeatures()
	{
		ConstrainCursor(false);

#if WIN
#elif MAC
		LowLevel_APIs.ShowMenuBar();
#endif
	}

	public static void ConstrainCursor(bool toOrNotTo = true)
	{
		const double x_factor = 0.25;
		const double y_factor = 0.06;

		var x = Screen.Width;
		var y = Screen.Height;

#if WIN
		var dimensions = new LowLevel_APIs.RECT
		{
			Left = (int)(x * x_factor),
			Top = (int)(y * (y_factor + 0.15)),
			Right = (int)(x - x * x_factor),
			Bottom = (int)(y - y * y_factor)
		};

		if (toOrNotTo)
			LowLevel_APIs.ClipCursor(ref dimensions);
		else
			LowLevel_APIs.ClipCursor(IntPtr.Zero);
#elif MAC
		double screenR = Screen.Width;
		double screenL = 0;
		double screenT = Screen.Height;
		double screenB = 0;

		// Calculating Offsets
		var offsetX = x * x_factor;
		var offsetY = y * y_factor;

		// Updating right and left boundaries
		screenR -= offsetX;
		screenL += offsetX;

		// Updating top and bottom boundaries
		screenT -= (offsetY * 4.5);
		screenB += (offsetY * 1.8);

		// Fetching current mouse location
		var nsPoint = LowLevel_APIs.GetClass("NSEvent");
		var selectR = LowLevel_APIs.RegisterName("mouseLocation");
		var cursPos = LowLevel_APIs.SendMessage_CGPoint(nsPoint, selectR);

		bool isCursorInRestrictedArea = false;
		var allowedPositionX = cursPos.x;
		var allowedPositionY = cursPos.y;

		// Check and adjust X position if needed
		if (cursPos.x < screenL || cursPos.x > screenR)
		{
			isCursorInRestrictedArea = true;
			allowedPositionX = cursPos.x < screenL ? screenL : screenR;
		}

		// Check and adjust Y position if needed
		if (cursPos.y < screenB || cursPos.y > screenT)
		{
			isCursorInRestrictedArea = true;
			allowedPositionY = cursPos.y < screenB ? screenB : screenT;
		}

		// If the cursor is already within the allowed area, just return
		if (!isCursorInRestrictedArea)
			return;

		// Flip Y coordinate when creating a custom mouse event in the desired position.
		var newLocation = new LowLevel_APIs.CoreGraphics.CGPoint(allowedPositionX, y - allowedPositionY);
		var cursorEvent = LowLevel_APIs.CoreGraphics.CGEventCreateMouseEvent(IntPtr.Zero, LowLevel_APIs.CoreGraphics.kCGEventMouseMoved, newLocation, 0);

		// Disassociate mouse cursor from real events.
		LowLevel_APIs.CoreGraphics.CGAssociateMouseAndMouseCursorPosition(false);

		// Post our fake event which moves the cursor
		LowLevel_APIs.CoreGraphics.CGEventPost(LowLevel_APIs.CoreGraphics.kCGHIDEventTap, cursorEvent);

		// Reassociate mouse cursor and real events.
		LowLevel_APIs.CoreGraphics.CGAssociateMouseAndMouseCursorPosition(true);
#endif
	}
}

public static partial class LowLevel_APIs
{
	public static readonly bool Is64Bit = IntPtr.Size == 8;

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

	// Pin to all Desktops
	// -------------------

	public const int GWL_EXSTYLE = -20;
	public const int WS_EX_TOOLWINDOW = 0x00000080;

	// Get Session's Username
	// ----------------------
	
	public enum WTS_INFO_CLASS
	{
		WTSUserName = 5,
		// Include Other parameters here, as needed
	}

	[LibraryImport("Wtsapi32.dll", EntryPoint = "WTSQuerySessionInformationA")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool WTSQuerySessionInformation(IntPtr hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

	[LibraryImport("Wtsapi32.dll")]
	public static partial void WTSFreeMemory(IntPtr pointer);

	[LibraryImport("Kernel32.dll")]
	public static partial uint WTSGetActiveConsoleSessionId();

	// Altering Window Styles
	// ----------------------

	public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
	{
		return Is64Bit ? GetWindowLongPtr64(hWnd, nIndex) : new IntPtr(GetWindowLongPtr32(hWnd, nIndex).ToInt32());
	}

	public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
	{
		return Is64Bit ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
	}

	[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
	private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
	private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
	private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

	// Cursor Clipping
	// ---------------

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ClipCursor(ref RECT rect);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ClipCursor(IntPtr rect);
#elif MAC

	// General APIs
	// ------------

	private const string ObjectiveCLibrary = "/usr/lib/libobjc.dylib";
	private const string AppKitLibrary = "/System/Library/Frameworks/AppKit.framework/AppKit";

	[DllImport(ObjectiveCLibrary, EntryPoint = "objc_getClass")]
	public extern static IntPtr GetClass(string className);

	[DllImport(ObjectiveCLibrary, EntryPoint = "sel_registerName")]
	public extern static IntPtr RegisterName(string selectorName);

	[DllImport(ObjectiveCLibrary, EntryPoint = "object_getIvar")]
	public static extern IntPtr GetInstanceVariable(IntPtr handle, IntPtr ivar);

	// Note for the Complication of 'objc_msgSend'
	// -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
	// 1stly, the Return Type is dependant on the Method Signature.
	// 2ndly, this methos has variadic arguments, of variadic types.
	// Thus, we need to create multiple overloads of this method.

	[DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
	public static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector);

	[DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend", CharSet = CharSet.Auto)]
	public static extern LowLevel_APIs.CoreGraphics.CGPoint SendMessage_CGPoint(IntPtr receiver, IntPtr selector);

	[DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
	public static extern void SendMessage(IntPtr receiver, IntPtr selector, int arg1);

	[DllImport(ObjectiveCLibrary, EntryPoint = "objc_msgSend_stret", CharSet = CharSet.Auto)]
	public static extern void SendMessage_stret_CGPoint(out LowLevel_APIs.CoreGraphics.CGPoint result, IntPtr receiver, IntPtr selector);

	// Menubar Management
	// ------------------

	public static void HideMenuBar()
	{
		var NSAppClass = GetClass("NSApplication");
		var sharedApplicationSelector = RegisterName("sharedApplication");
		var sharedApplication = SendMessage(NSAppClass, sharedApplicationSelector);
		var setPresentationOptionsSelector = RegisterName("setPresentationOptions:");
		SendMessage(sharedApplication, setPresentationOptionsSelector, 2); // NSApplicationPresentationHideMenuBar
	}

	public static void ShowMenuBar()
	{
		var NSAppClass = GetClass("NSApplication");
		var sharedApplicationSelector = RegisterName("sharedApplication");
		var sharedApplication = SendMessage(NSAppClass, sharedApplicationSelector);
		var setPresentationOptionsSelector = RegisterName("setPresentationOptions:");
		SendMessage(sharedApplication, setPresentationOptionsSelector, 0); // NSApplicationPresentationDefault
	}

	// Cursor Restriction
	// ------------------

	public static class CoreGraphics
	{
		public const int kCGEventMouseMoved = 5;
		public const int kCGHIDEventTap = 0;

		[StructLayout(LayoutKind.Sequential)]
		public struct CGPoint
		{
			public double x;
			public double y;

			public CGPoint(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		public static extern int CGAssociateMouseAndMouseCursorPosition(bool connected);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		public static extern IntPtr CGEventCreateMouseEvent(IntPtr source, int mouseType, CGPoint mouseCursorPosition, int mouseButton);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		public static extern void CGEventPost(int tapLocation, IntPtr eventRef);

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		public static extern int CGWarpMouseCursorPosition(CGPoint newCursorPosition);

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		public static extern int CGDisplayPixelsWide(int displayId);

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		public static extern int CGDisplayPixelsHigh(int displayId);

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
		public static extern int CGMainDisplayID();
	}
#endif
}