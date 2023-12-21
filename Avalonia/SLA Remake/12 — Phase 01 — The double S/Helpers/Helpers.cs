using Avalonia.Interactivity;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SLA_Remake;

public static class Utility
{
	public static IPAddress GetIP() => 
		Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(AllowedIP);

	private static bool AllowedIP(IPAddress ip) =>
		ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
		ip.ToString() != "127.0.0.1";
}

public static class CrossUtility
{
	// CrossUtility provides a uniform API for all the OS-specific APIs.
	// Each method is implemented separately for both Windows and MacOS.

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
		var username = Controls.EnableOriginalUser
			? Environment.UserName
			: Controls.PlaceholderUsername;
		if (!deepFetch) return username;
#if WIN
		// Note for the Complication in-case on Windows
		// --------------------------------------------
		// Here, Username is fetched of the currently Logged-In User.
		// The Environment.UserName is not very reliable in this case,
		// as it returns the Username of the User who started the App.
		// So, Username is fetched, from the currently active Session.
		// In-case of fetch-failure, the Environment.UserName is used.

		var sessionId = LowLevel_APIs.WTSGetActiveConsoleSessionId();

		if (sessionId == 0xFFFFFFFF)
			return username;

		if (!LowLevel_APIs.WTSQuerySessionInformation(IntPtr.Zero, sessionId, LowLevel_APIs.WTS_INFO_CLASS.WTSUserName, out var buffer, out _))
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
		const string cmd = "rundll32.exe";
		const string arg = "user32.dll,LockWorkStation";
#elif MAC
		const string cmd = "osascript";
		const string arg = "-e 'tell application \"System Events\" to keystroke \"q\" using {command down, control down, option down}'";
#endif
		var detail = new ProcessStartInfo
		{
			FileName = cmd,
			Arguments = arg,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		Process.Start(detail);
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
		var plistPath = System.IO.Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "~", "Library", "LaunchAgents");
		var plistFile = System.IO.Path.Combine(plistPath, plistName);

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

		System.IO.Directory.CreateDirectory(plistPath);
		System.IO.File.WriteAllText(plistFile, plistContent);

		var startInfo = new ProcessStartInfo
		{
			FileName = "/bin/bash",
			Arguments = $"-c \"launchctl list | grep {myAppName}\"",
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true,
		};

		using var process = Process.Start(startInfo);
		process?.WaitForExit();

		if (process?.ExitCode == 0) return;
		startInfo.Arguments = $"-c \"launchctl load {plistFile}\"";
		Process.Start(startInfo);
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
		var detail = new ProcessStartInfo
		{
			FileName = "/bin/bash",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			Arguments = "-c \"ioreg -c IOHIDSystem | awk '/HIDIdleTime/ {print $NF/1000000000; exit}'\""
		};

		using var process = Process.Start(detail);
		var output = process?.StandardOutput.ReadToEnd() ?? "0";
		process?.WaitForExit();

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

	public static Models.CurrentAppInformation GetActiveProcessInfo()
	{
#if WIN
		var handle = LowLevel_APIs.GetForegroundWindow();
		var thread = LowLevel_APIs.GetWindowThreadProcessId(handle, out var processId);
		var method = Process.GetProcessById((int)processId);
		var buffer = new StringBuilder(LowLevel_APIs.MAX_TITLE_LENGTH);
		var result = LowLevel_APIs.GetWindowText(handle, buffer, LowLevel_APIs.MAX_TITLE_LENGTH);

		return new(processId, buffer.ToString(), method.ProcessName);
#elif MAC
		var detail = new ProcessStartInfo
		{
			FileName = "/bin/bash",
			Arguments = "-c \"osascript -e 'tell application \\\"System Events\\\" to get the title of every window of (every process whose frontmost is true)' && osascript -e 'tell application \\\"System Events\\\" to unix id of (every process whose frontmost is true)'\"",
			RedirectStandardOutput = true,
			UseShellExecute = false
		};

		var method = Process.Start(detail);
		method?.WaitForExit();

		var output = (method?.StandardOutput.ReadToEnd().Trim() ?? string.Empty).Split('\n');
		var banner = output.FirstOrDefault();
		var result = uint.TryParse(output.LastOrDefault(), out var unixID);
		var p_name = result ? Process.GetProcessById((int)unixID).ProcessName : string.Empty;

		return new(unixID, banner, p_name);
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

	public static void CaptureAndSaveScreenshot(string customPath = null)
	{
		const long imgQuality = 30L;
		var filename = $"screenshot-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}{Controls.ImagesExtension}";
		var savePath = System.IO.Path.Combine(customPath ?? Controls.HomeFolder, Controls.ScreenshotFolder);

		System.IO.Directory.CreateDirectory(savePath);
		var saveFile = System.IO.Path.Combine(savePath, filename);
#if WIN
		using var BMP = new System.Drawing.Bitmap(Screen.Wide, Screen.High);
		using var GFX = System.Drawing.Graphics.FromImage(BMP);
		GFX.CopyFromScreen(0, 0, 0, 0, BMP.Size);

		var encoder = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
		var e_param = new System.Drawing.Imaging.EncoderParameters { Param = { [0] = new(System.Drawing.Imaging.Encoder.Quality, imgQuality) } };

		BMP.Save(saveFile, encoder, e_param);
#elif MAC    
		var temporary = System.IO.Path.Combine(savePath, $"{Guid.NewGuid()}.png");
		var startInfo = new ProcessStartInfo
		{
			FileName = "/usr/sbin/screencapture",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			Arguments = $"-x \"{temporary}\""  // -x to mute the sound
		};
		Process.Start(startInfo)?.WaitForExit();
	
		startInfo.FileName = "/usr/bin/sips";
		startInfo.Arguments = $"-s format jpeg -s formatOptions {imgQuality} \"{temporary}\" --out \"{saveFile}\"";
		var p = Process.Start(startInfo); p?.WaitForExit();

		if (p?.ExitCode != 0) return;
		System.IO.File.Delete(temporary);
#endif
	}
}

public static partial class LowLevel_APIs
{
	// All the Native APIs are defined here.
	// Proceed with caution, altering these.

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

	[GeneratedRegex(@"^\s*at", RegexOptions.Multiline)]
	public static partial Regex StackTraceRegex();

#if WIN

	#region Fetching Idle Time

	public struct LASTINPUTINFO
	{
		public uint cbSize;
		public uint dwTime;
	}

	[LibraryImport(Library.User32)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetLastInputInfo(ref LASTINPUTINFO plii);

	#endregion

	#region Pin to all Desktops

	public const int GWL_EXSTYLE = -20;
	public const int WS_EX_TOOLWINDOW = 0x00000080;

	#endregion

	#region Get Session's Username

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

	#endregion

	#region Altering Window Styles

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

	#endregion

	#region Cursor Clipping

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

	#endregion

	#region Info-Tracking

	[LibraryImport("user32.dll")]
	public static partial IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[LibraryImport("user32.dll", SetLastError = true)]
	public static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

	public const int MAX_TITLE_LENGTH = 256;

	#endregion

#elif MAC

	#region General APIs

	[LibraryImport(Library.ObjectiveC, EntryPoint = "objc_getClass", StringMarshalling = StringMarshalling.Utf8)]
	public static partial IntPtr GetClass([MarshalAs(UnmanagedType.LPUTF8Str)] string className);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "sel_registerName", StringMarshalling = StringMarshalling.Utf8)]
	public static partial IntPtr RegisterName([MarshalAs(UnmanagedType.LPUTF8Str)] string selectorName);

	[LibraryImport(Library.ObjectiveC, EntryPoint = "object_getIvar")]
	public static partial IntPtr GetInstanceVariable(IntPtr handle, IntPtr ivar);

	// Note for the Complication of 'objc_msgSend'
	// -  -  -  -  -  -  -  -  -  -  -  -  -  -  -
	// 1stly, its Return Type is dependent on the Method Signature.
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

	#endregion

	#region Menubar Management

	private static void SetPresentation(System.Collections.Generic.IEnumerable<int> options)
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

	#endregion

	#region Cursor Clipping

	public static partial class CoreGraphics
	{
		public const int kCGEventMouseMoved = 5;
		public const int kCGHIDEventTap = 0;

		[StructLayout(LayoutKind.Sequential)]
		public struct CGPoint(double x, double y)
		{
			public double x = x;
			public double y = y;
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

	#endregion

#endif
}