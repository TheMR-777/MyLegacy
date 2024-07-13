using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Playground;

public class CurrentAppInformation(uint pid, string title, string name, Image? icon)
{
	public uint ProcessSerial { get; set; } = pid;
	public string WindowTitle { get; set; } = title;
	public string ProcessName { get; set; } = name;
	public Image? Icon { get; set; } = icon;
}

public partial class Program
{
	public static CurrentAppInformation GetActiveProcessInfo()
	{
#if WIN
		var handle = LowLevel_APIs.GetForegroundWindow();
		var thread = LowLevel_APIs.GetWindowThreadProcessId(handle, out var processId);
		var method = Process.GetProcessById((int)processId);
		var buffer = new System.Text.StringBuilder(LowLevel_APIs.MAX_TITLE_LENGTH);
		var result = LowLevel_APIs.GetWindowText(handle, buffer, LowLevel_APIs.MAX_TITLE_LENGTH);

		Image? icon = null;
		try
		{
			var iconPath = Icon.ExtractAssociatedIcon(method.MainModule.FileName);
			icon = new Bitmap(iconPath.ToBitmap(), new Size(32, 32));
		}
		catch { }

		return new(processId, buffer.ToString(), method.ProcessName, icon);
#elif MAC
	var detail = new ProcessStartInfo
	{
		FileName = "/bin/bash",
		Arguments = "-c \"osascript -e 'tell application \\\"System Events\\\" to get the title of every window of (every process whose frontmost is true)' && osascript -e 'tell application \\\"System Events\\\" to unix id of (every process whose frontmost is true)' && osascript -e 'tell application \\\"System Events\\\" to get the file of (every process whose frontmost is true)'\"",
		RedirectStandardOutput = true,
		UseShellExecute = false
	};

	var method = Process.Start(detail);
	method?.WaitForExit();

	var output = (method?.StandardOutput.ReadToEnd().Trim() ?? string.Empty).Split('\n');
	var banner = output.FirstOrDefault() ?? string.Empty;
	var result = uint.TryParse(output[1], out var unixID);
	var p_name = result ? Process.GetProcessById((int)unixID).ProcessName : Configuration.NotAvailableOrFound;
	var iconPath = output.LastOrDefault() ?? Configuration.NotAvailableOrFound;

	Image icon = null;
	try
	{
		var iconFilePath = $"/usr/bin/sips -s format png --resampleWidth 16 {iconPath} --out icon.png"; // Convert to small size PNG
		var iconProcess = Process.Start("/bin/bash", $"-c \"{iconFilePath}\"");
		iconProcess?.WaitForExit();
		icon = Image.FromFile("icon.png");
	}
	catch (Exception)
	{
		icon = null;
	}

	return new(unixID, banner, p_name, icon);
#endif
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

		[GeneratedRegex(@"^\s*at (?<namespace>(SLA_Remake\.|Avalonia\.|System\.)?)?(?<method>.+?)( in (?<filepath>.+\\)?(?<filename>[^\\]+):line (?<line>\d+))?\s*$", RegexOptions.Multiline)]
		public static partial Regex StackTraceMarksman();

#if WIN

		#region Info-Tracking

		[LibraryImport("user32.dll")]
		public static partial IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

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

	static void Main()
	{
		var info = GetActiveProcessInfo();

		Console.WriteLine($"Process Name: {info.ProcessName}");
		Console.WriteLine($"Window Title: {info.WindowTitle}");

		// Save the icon to a file

		info.Icon.Save("icon.png");
	}
}
