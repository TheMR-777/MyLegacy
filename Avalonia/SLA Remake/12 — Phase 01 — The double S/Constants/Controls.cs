namespace SLA_Remake;

public static class Controls
{
	// Command and Control
	// -------------------

	private const bool yes = true;
	private const bool no = false;
	
	public const bool EnableTheInternet = yes;		// no: The Internet will not be used anywhere
	public const bool EnablePrimeGuard = yes;       // no: The PrimeGuard won't initiate (which repeatedly regains application control, forcefully) 
    public const bool EnableOriginalUser = yes;		// no: The placeholder username will be used everywhere
	public const bool EnableNewerVersion = yes;		// no: The newer version will be considered as older
	public const bool EnableLogOnDiscord = yes;		// no: The Discord Reporting will be disabled
	public const bool EnableLoggingOnAPI = yes;		// no: The LogEntry will not be sent to the API
	public const bool EnableCacheLogging = yes;     // no: The LogEntry will not be saved in the Database

	// Other Constants
	// ---------------

	public const string PlaceholderUsername = "TEST";
	public const string ApplicationVersion = EnableNewerVersion ? "3.0.0.00" : "2.0.0.01";
	public const string DiscordWebhookURL = "https://discord.com/api/webhooks/1172483585698185226/M1oWUKwwl-snXr6sHTeAQoKYQxmg4JVg-tRKkqUZ1gSuYXwsV5Q9DhZj00mMX0_iui6d";
}