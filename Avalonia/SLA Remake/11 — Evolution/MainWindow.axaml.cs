using Avalonia;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dapper;
using ReasonType = System.Tuple<uint, bool, string>;

namespace SLA_Remake;

public static class Constants
{
	// Command and Control
	// -------------------

	private const bool yes = true;
	private const bool no = false;
	
	public const bool EnableTheInternet = yes;		// no: The Internet will not be used anywhere
	public const bool EnablePrimeGuard = yes;		// no: The PrimeGuard won't initiate
	public const bool EnableOriginalUser = no;		// no: The placeholder username will be used everywhere
	public const bool EnableNewerVersion = no;		// no: The newer version will be considered as older
	public const bool EnableLogOnDiscord = yes;		// no: The Discord Reporting will be disabled
	public const bool EnableLoggingOnAPI = yes;		// no: The LogEntry will not be sent to the API
	public const bool EnableCacheLogging = yes;     // no: The LogEntry will not be saved in the Database

	// Other Constants
	// ---------------

	public const string PlaceholderUsername = "TEST";
	public const string ApplicationVersion = EnableNewerVersion ? "3.0.0.00" : "2.0.0.01";
	public const string DiscordWebhookURL = "https://discord.com/api/webhooks/1172483585698185226/M1oWUKwwl-snXr6sHTeAQoKYQxmg4JVg-tRKkqUZ1gSuYXwsV5Q9DhZj00mMX0_iui6d";
}

public partial class MainWindow : Window
{
	private readonly bool IsDesigning = Design.IsDesignMode;
	private const double _reasonsBoxHeight = 35.00 / 100.00;
	private readonly TimeSpan _idleAllowed = TimeSpan.FromMinutes(5);
	private readonly TimeSpan _postmanTime = TimeSpan.FromSeconds(17);
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

	public static List<ReasonType> ReasonsAll { get; } =
	[
		new(1, false, "Start of the Day"),
		new(2, false, "System Reboot"),
		new(3, false, "Break"),
		new(4, false, "Rest Room"),
		new(5, false, "Namaz"),
		new(6, false, "Lunch"),
		new(7, false, "Dinner"),
		new(8, false, "Late Leave"),
		new(9, false, "Short Leave"),
		new(10, false, "Half Leave"),
		new(11, true, "On Official Call / Duty"),
		new(12, true, "Filing / Paper Work"),
		new(13, true, "Official Outgoing"),
		new(14, true, "Official Gathering"),
		new(15, true, "Seminar / Training / Presentation / Lecture"),
		new(16, true, "Departmental Discussion"),
		new(17, true, "Formal Meeting"),
		new(18, true, "Informal Meeting"),
		new(19, true, "Other")
	];

	public static readonly List<Key> RestrictedKeys =
	[
		// Alt, Ctrl, etc. are being handled separately
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
	];

	public static readonly List<KeyModifiers> RestrictedModifiers =
	[
		KeyModifiers.Alt,       KeyModifiers.Control,       KeyModifiers.Meta,
	];

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
		ReasonsAll.Sort((x, y) => string.Compare(x.Item3, y.Item3, StringComparison.OrdinalIgnoreCase));

		// Timer Initialization
		{
			_backgroundTimer.Tick += BackgroundTimer_Tick;
			_primeGuardTimer.Tick += !Constants.EnablePrimeGuard || IsDesigning ? NoOperation : PrimeGuard_Tick;
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

		// Starting Postman
		{
			if (!Constants.EnableLoggingOnAPI) return;
			System.Threading.Tasks.Task.Run(async () =>
			{
				while (true)
				{
					await System.Threading.Tasks.Task.Delay(_postmanTime);
					PostmanJob();
				}
			});
		}

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
		_loginButton.Click += LoginButton_Click;
		_reasonsBox.SelectionChanged += ReasonsBox_SelectionChanged;
		_reasonMore.TextChanged += ReasonsDetail_TextChanged;
	}

	private void ResetFields()
	{
		_reasonsBox.SelectedIndex = -1;
		_reasonMore.Text = string.Empty;
		_reasonMore.IsVisible = false;
		_loginButton.IsEnabled = false;
	}

	private void ForceActivate(object _ = null, object __ = null)
	{
		Activate();
		this.BringIntoView();
	}

	private static void PostmanJob(object _ = null, object __ = null)
	{
		try
		{
			if (!Constants.EnableCacheLogging) return;

			if (!WebAPI.VerifyDatabase()) return;
			var entries = Database.GetSavedEntries();

			if (entries.Count == 0) return;
			var success = WebAPI.SendEntries(entries);

			if (!success) return;
			Database.Clear();
		}
		catch (Exception e)
		{
			WebAPI.L1_RegisterException(e);
		}
	}

	private static void NoOperation(object _ = null, object __ = null) { }

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

		_backgroundTimer.Tick += ForegroundTimer_TickTask;
		Deactivated += Constants.EnablePrimeGuard ? ForceActivate : NoOperation;
		_primeGuardTimer.Start();

		// Height Adjustment
		// -----------------

		_reasonsBox.MaxHeight = CrossUtility.Screen.High * _reasonsBoxHeight;

		if (IsDesigning) return;

		// Database Logging
		// ----------------

		var entry = Database.LogEntry.Create(login: false);
		Database.Save(entry);
	}

	public static void WindowClosing(object _, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	public static void WindowClosed(object _, EventArgs __)
	{
		// Reinitiating measures can be Taken here
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
		_loginButton.Content = $@"Login - {_exitTime:hh\:mm\:ss}";
		_debugButton.Content = $@"Debug - {CrossUtility.GetIdleTime():hh\:mm\:ss}";
	}

	private void LoginButton_Click(object _, RoutedEventArgs __)
	{
		// Stopping Timers
		// ---------------

		_backgroundTimer.Tick -= ForegroundTimer_TickTask;
		Deactivated -= ForceActivate;
		_primeGuardTimer.Stop();

		// _exitTime Interval can be handled here

		if (IsDesigning) return;

		// Database Logging
		// ----------------

		var entry = Database.LogEntry.Create(
			login: true,
			reasonDetail: _reasonMore.Text,
			reason: (ReasonType)_reasonsBox.SelectedItem
		);
		Database.Save(entry);

		// Release Restrictions
		// --------------------

		Hide();
		CrossUtility.AllowOSFeatures();
		ResetFields();
	}

	private void ReasonsBox_SelectionChanged(object _, SelectionChangedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not ReasonType kvp) return;
		_reasonMore.IsVisible = kvp.Item2;
		_loginButton.IsEnabled = !(kvp.Item2 && string.IsNullOrWhiteSpace(_reasonMore.Text));
	}

	private void ReasonsDetail_TextChanged(object _, RoutedEventArgs __)
	{
		if (_reasonsBox.SelectedItem is not ReasonType) return;
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

	public static bool SendEntries(List<Database.LogEntry> entries)
	{
		if (!ConnectedToInternet()) return false;

		using var res = PostDataTo(logEntryLogging, Serialize(entries));
		return res.Content.ReadAsStringAsync().Result == "1";
	}

	public static void L1_RegisterException(Exception x)
	{
		if (!Constants.EnableLogOnDiscord) return;

		// Fetching Footprints
		// -------------------

		var footprints = new StackTrace().GetFrames()!
			.Select(frame => frame.GetMethod()!)
			.Select(methodReference => $"{methodReference.DeclaringType!}.{methodReference.Name}({string.Join(", ", methodReference.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray())})").ToList();

		// Fetching User Information
		// -------------------------

		var userInf = new[]
		{
			CrossUtility.GetCurrentUser(deepFetch: true),
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
							{ "value", $"```{string.Join(" < ", footprints)}```" },
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

		PostDataTo(Constants.DiscordWebhookURL, System.Text.Json.JsonSerializer.Serialize(body));
	}

	// Web-Utilities
	// -------------

	public static bool ConnectedToInternet()
	{
		if (!Constants.EnableTheInternet) return false;

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

	private static HttpResponseMessage PostDataTo(string endPoint, string data)
	{
		using var req = new HttpRequestMessage(HttpMethod.Post, endPoint)
		{
			Content = new StringContent(
				data,
				new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
			)
		};
		return _webClient.SendAsync(req).Result;
	}

	private static string Serialize(IEnumerable<Database.LogEntry> entries)
	{
		var entriesFormatted = entries.Select(entry =>
		{
			var properties = entry.GetType().GetProperties();
			var values = properties.Select(property => property.GetValue(entry, null));
			return string.Join("|", values);
		});

		var now = DateTime.Now;
		var req = new
		{
			UserName = CrossUtility.GetCurrentUser() + '~' + Environment.MachineName,
			logDate = now.Date.ToString("dd/MM/yyyy HH:mm:ss") + "~WQoCW/gL8O/+pi0RP2l6xg==",
			LogDateTimeStamp = now.ToString("dd/MM/yyyy HH:mm:ss"),
			version = Constants.ApplicationVersion,
			data = entriesFormatted
		};

		return System.Text.Json.JsonSerializer.Serialize(req, new System.Text.Json.JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
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

		public static LogEntry Create(bool login = true, string reasonDetail = null, ReasonType reason = null) => new()
		{
			// Constants
			// ---------

			UserId = "1",
			UserName = CrossUtility.GetCurrentUser(),
			UserIp = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(AllowedIP)?.ToString(),
			LogDate = DateTime.Now.Date.ToOADate().ToString(CultureInfo.InvariantCulture),
			UserPCName = Dns.GetHostName(),
			UserDisplayName = "USER",

			// Variables
			// ---------

			LogInTime = login
				? DateTime.Now.ToString("HH:mm")
				: null,
			LogOutTime = login
				? null
				: DateTime.Now.ToString("HH:mm"),
			LogFlag = login 
				? "0" 
				: "1",
			Reason = login
				? RemoveSpecialCharacters(reason.Item2
					? reasonDetail?.Trim()
					: BackwardCompatibility.GetCompatibleReasonText(reason))
				: null,
			ReasonType = login
				? BackwardCompatibility.GetCompatibleReasonText(reason)
				: null,
			ReasonId = login
				? reason.Item1.ToString(CultureInfo.InvariantCulture)
				: "0",
			LogSide = login
				? "i"
				: "o"
		};

		private static bool AllowedIP(IPAddress ip) =>
			ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
			ip.ToString() != "127.0.0.1";

		private static string RemoveSpecialCharacters(string str) =>
			new(str.Where(c => 
				char.IsLetterOrDigit(c) || 
				char.IsWhiteSpace(c) || 
				new[] { '.', '-', '_' }.Contains(c))
			.ToArray());
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

		public static string SelectAll()
		{
			var query = $"SELECT * FROM {_name};";
			return query;
		}

		public static string ClearTable()
		{
			var query = $"DELETE FROM {_name};";
			return query;
		}
	}

	private const string DatabaseFullName = "Database.sqlite";
	private static readonly string DatabaseLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFullName);
	private static readonly string ConnectionString = $"Data Source={DatabaseLocation};Version=3;";
	private static readonly System.Data.SQLite.SQLiteConnection Connection = new(ConnectionString);

	public static int Save(LogEntry entry)
	{
		if (!Constants.EnableCacheLogging) return 0;

		Connection.Open();
		var query = QueryBuilder.GenerateTable();
		Connection.Execute(query);

		query = QueryBuilder.Insert();
		var result = Connection.Execute(query, entry);

		Connection.Close();
		return result;
	}

	public static List<LogEntry> GetSavedEntries()
	{
		if (!Constants.EnableCacheLogging) return new();

		Connection.Open();
		var query = QueryBuilder.SelectAll();
		var result = Connection.Query<LogEntry>(query).ToList();
		Connection.Close();
		return result;
	}

	public static int Clear()
	{
		if (!Constants.EnableCacheLogging) return 0;

		Connection.Open();
		var query = QueryBuilder.ClearTable();
		var result = Connection.Execute(query);
		Connection.Close();
		return result;
	}
}

public static class CrossUtility
{
	public static class Screen
	{
#if WIN
		public static readonly int Wide = LowLevel_APIs.GetSystemMetrics(0);
		public static readonly int High = LowLevel_APIs.GetSystemMetrics(1);
#elif MAC
		public static readonly int Wide = LowLevel_APIs.CoreGraphics.CGDisplayPixelsWide(LowLevel_APIs.CoreGraphics.CGMainDisplayID());
		public static readonly int High = LowLevel_APIs.CoreGraphics.CGDisplayPixelsHigh(LowLevel_APIs.CoreGraphics.CGMainDisplayID());
#endif
	}

	public static string GetCurrentUser(bool deepFetch = false)
	{
		var username = Constants.EnableOriginalUser 
			? Environment.UserName 
			: Constants.PlaceholderUsername;
		if (!deepFetch) return username;
#if WIN
		// Note for the Complication in-case on Windows
		// --------------------------------------------
		// Here, Username is fetched of the currently Logged-In User.
		// The Environment.UserName is not reliable in this case,
		// as it returns the Username of the User who started the App.
		// So, Username is fetched, from the currently active Session.
		// In-case of fetch-failure, the Environment.UserName is used.

		var sessionId = LowLevel_APIs.WTSGetActiveConsoleSessionId();

		if (sessionId == 0xFFFFFFFF)
			return username;

		if (!LowLevel_APIs.WTSQuerySessionInformation(IntPtr.Zero, sessionId, LowLevel_APIs.WTS_INFO_CLASS.WTSUserName,	out var buffer, out _)) 
			return username;

		username = Marshal.PtrToStringAnsi(buffer) ?? username;
		LowLevel_APIs.WTSFreeMemory(buffer);
#elif MAC
#endif
		return username;
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

	public static void RegisterForStartup()
	{
#if WIN
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
		var processModule = Process.GetCurrentProcess().MainModule;
		if (processModule == null) return;
		var exeLocation = processModule.FileName;
		var appName = System.IO.Path.GetFileNameWithoutExtension(exeLocation);
		key.SetValue(appName, exeLocation);

#elif MAC
		var myAppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
		var plistName = $"{myAppName}.plist";
		var plistPath = $"{Environment.GetEnvironmentVariable("HOME")}/Library/LaunchAgents/{plistName}";

		var plistContent = new System.Text.StringBuilder()
				.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
				.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">")
				.AppendLine("<plist version=\"1.0\">")
				.AppendLine("<dict>")
				.AppendLine("\t<key>Label</key>")
				.AppendLine($"\t<string>{myAppName}</string>")
				.AppendLine("\t<key>ProgramArguments</key>")
				.AppendLine("\t<array>")
				.AppendLine($"\t\t<string>{System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, myAppName)}</string>")
				.AppendLine("\t</array>")
				.AppendLine("\t<key>RunAtLoad</key>")
				.AppendLine("\t<true/>")
				.AppendLine("</dict>")
				.AppendLine("</plist>").ToString();

		System.IO.File.WriteAllText(plistPath, plistContent);

		var processStartInfo = new ProcessStartInfo
		{
			FileName = "/bin/bash",
			Arguments = $"-c \"launchctl list | grep {myAppName}\"",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
		};

		var process = new Process { StartInfo = processStartInfo };

		process.Start();
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			processStartInfo.Arguments = $"-c \"launchctl load {plistPath}\"";
			process = new Process { StartInfo = processStartInfo };
			process.Start();
		}
#endif
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
		LowLevel_APIs.MakeAppStrict();
#endif
	}

	public static void AllowOSFeatures()
	{
		ConstrainCursor(false);

#if WIN
#elif MAC
		LowLevel_APIs.MakeAppNormal();
#endif
	}

	public static void ConstrainCursor(bool toOrNotTo = true)
	{
		const double x_factor = 0.25;
		const double y_factor = 0.06;

		var x = Screen.Wide;
		var y = Screen.High;

#if WIN
		const double adjust_y = 0.15;

		var dimensions = new LowLevel_APIs.RECT
		{
			Left = (int)(x * x_factor),
			Top = (int)(y * (y_factor + adjust_y)),
			Right = (int)(x - x * x_factor),
			Bottom = (int)(y - y * y_factor)
		};

		if (toOrNotTo)
			LowLevel_APIs.ClipCursor(ref dimensions);
		else
			LowLevel_APIs.ClipCursor(IntPtr.Zero);
#elif MAC
		// Adjustments
		const double adjust_x = 4.5;
		const double adjust_y = 1.8;

		double screenR = Screen.Wide;
		double screenL = 0;
		double screenT = Screen.High;
		double screenB = 0;

		// Calculating Offsets
		var offsetX = x * x_factor;
		var offsetY = y * y_factor;

		// Updating right and left boundaries
		screenR -= offsetX;
		screenL += offsetX;

		// Updating top and bottom boundaries
		screenT -= (offsetY * adjust_x);
		screenB += (offsetY * adjust_y);

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

public static class BackwardCompatibility
{
	private static readonly Dictionary<uint, string> OldReasons = new()
	{
		// The Dictionary is being used, to maintain the order (of ID) of the Reasons.
		// The order is crucial, as it is by the order of IDs located in the Database.

		{ 1, "Day Start" },
		{ 2, "System Restart/Shutdown" },
		{ 3, "Break" },
		{ 4, "Rest Room" },
		{ 5, "Namaz" },
		{ 6, "Lunch" },
		{ 7, "Dinner" },
		{ 8, "Late Leave" },
		{ 9, "Short Leave" },
		{ 10, "Half Leave" },
		{ 11, "On Official Call / On Seat" },
		{ 12, "Filling / Paper Work" },
		{ 13, "Official Outgoing" },
		{ 14, "Company /Departmental Gathering" },
		{ 15, "Seminar/Lecture/Presentation/Training" },
		{ 16, "Departmental Discussion (Purpose)" },
		{ 17, "Formal Meeting" },
		{ 18, "Informal Meeting" },
		{ 19, "Other" }
	};

	public static string GetCompatibleReasonText(ReasonType reason)
	{
		return OldReasons.TryGetValue(reason.Item1, out var text) ? text : reason.Item3;
	}
}

public static partial class LowLevel_APIs
{
	private static readonly bool Is64Bit = IntPtr.Size == 8;

	private static class Library
	{
		public const string User32 = "user32.dll";
		public const string Kernel32 = "Kernel32.dll";
		public const string WtsAPI32 = "Wtsapi32.dll";

		public const string ObjectiveC = "/usr/lib/libobjc.dylib";
		public const string AppKit_Lib = "/System/Library/Frameworks/AppKit.framework/AppKit";
		public const string AppService = "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";
		public const string CoreGraphs = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
	}

#if WIN

	// Fetching Idle Time
	// ------------------

	public struct LASTINPUTINFO
	{
		public uint cbSize;
		public uint dwTime;
	}

	[LibraryImport(Library.User32)]
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

	[LibraryImport(Library.WtsAPI32, EntryPoint = "WTSQuerySessionInformationA")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool WTSQuerySessionInformation(IntPtr hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

	[LibraryImport(Library.WtsAPI32)]
	public static partial void WTSFreeMemory(IntPtr pointer);

	[LibraryImport(Library.Kernel32)]
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

	// Note for no-LibraryImport
	// - - - - - - - - - - - - -
	// Since, the EntryPoint is different for 32-bit and 64-bit,
	// we need to create multiple overloads of the same methods.

	[DllImport(Library.User32, EntryPoint = "GetWindowLong")]
	private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

	[DllImport(Library.User32, EntryPoint = "GetWindowLongPtr")]
	private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

	[DllImport(Library.User32, EntryPoint = "SetWindowLong")]
	private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

	[DllImport(Library.User32, EntryPoint = "SetWindowLongPtr")]
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

	[LibraryImport(Library.User32)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ClipCursor(ref RECT rect);

	[LibraryImport(Library.User32)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ClipCursor(IntPtr rect);

	[LibraryImport(Library.User32)]
	public static partial int GetSystemMetrics(int nIndex);
#elif MAC

	// General APIs
	// ------------

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_getClass", StringMarshalling = StringMarshalling.Utf8)]
	public static partial IntPtr GetClass([MarshalAs(UnmanagedType.LPUTF8Str)] string className);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "sel_registerName", StringMarshalling = StringMarshalling.Utf8)]
	public static partial IntPtr RegisterName([MarshalAs(UnmanagedType.LPUTF8Str)] string selectorName);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "object_getIvar")]
	public static partial IntPtr GetInstanceVariable(IntPtr handle, IntPtr ivar);

	// Note for the Complication of 'objc_msgSend'
	// -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
	// 1stly, its Return Type is dependant on the Method Signature.
	// 2ndly, the Method has variadic arguments, of variadic types.
	// Hence, we need to create multiple overloads of this method.

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_msgSend")]
	public static partial IntPtr SendMessage(IntPtr receiver, IntPtr selector);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_msgSend")]
	public static partial CoreGraphics.CGPoint SendMessage_CGPoint(IntPtr receiver, IntPtr selector);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_msgSend")]
	public static partial void SendMessage(IntPtr receiver, IntPtr selector, int arg1);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_msgSend_stret")]
	public static partial void SendMessage_stret_CGPoint(out CoreGraphics.CGPoint result, IntPtr receiver, IntPtr selector);

	private static IntPtr GetApplicationReference()
	{
		var NSAppClass = GetClass("NSApplication");
		var SelectApps = RegisterName("sharedApplication");
		return SendMessage(NSAppClass, SelectApps);
	}

	// Menubar Management
	// ------------------

	private static void SetPresentation(int[] options)
	{
		var SharedApps = GetApplicationReference();
		var GetOptions = RegisterName("setPresentationOptions:");
		SendMessage(SharedApps, GetOptions, options.Aggregate(0, (x, y) => x | (1 << y)));
	}

	// Presentation Options Guide:
	// - - - - - - - - - - - - - -
	// 0        : Reset
	// 1 << 00  :  x Auto-Hide Dock
	// 1 << 01  : Hide Dock
	// 1 << 02  :  x Auto-Hide Menu Bar
	// 1 << 03  : Hide Menu Bar
	// 1 << 04  : Disable Apple-Menu
	// 1 << 05  : Disable Process-Switching
	// 1 << 06  : Disable Force-Quit
	// 1 << 07  : Disable Session-Termination
	// 1 << 08  : Disable Hide-Application
	// 1 << 09  : Disable Menubar Translucency
	// 1 << 10  :  x Fullscreen
	// 1 << 11  : Auto-Hide Toolbar

	public static void MakeAppStrict() => SetPresentation([1, 3, 4, 5, 6, 7, 8, 9, 11]);
	public static void MakeAppNormal() => SetPresentation([0, 6, 7, 8]);

	// Cursor Restriction
	// ------------------

	public static partial class CoreGraphics
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

		[LibraryImport(Library.AppService)]
		public static partial int CGAssociateMouseAndMouseCursorPosition([MarshalAs(UnmanagedType.Bool)] bool connected);

		[LibraryImport(Library.AppService)]
		public static partial int CGMainDisplayID();

		[LibraryImport(Library.AppService)]
		public static partial IntPtr CGEventCreateMouseEvent(IntPtr source, int mouseType, CGPoint mouseCursorPosition, int mouseButton);

		[LibraryImport(Library.AppService)]
		public static partial void CGEventPost(int tapLocation, IntPtr eventRef);

		[LibraryImport(Library.CoreGraphs)]
		public static partial int CGWarpMouseCursorPosition(CGPoint newCursorPosition);

		[LibraryImport(Library.CoreGraphs)]
		public static partial int CGDisplayPixelsWide(int displayId);

		[LibraryImport(Library.CoreGraphs)]
		public static partial int CGDisplayPixelsHigh(int displayId);
	}
#endif
}
