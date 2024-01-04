using System;

namespace SLA_Remake.Models;

public class ScreenshotEntry
{
	// This class contains required information about the screenshot
	// that is to be saved in the Database after it is posted at S3.
	// Be advised, these properties must NOT be RENAMED, as they are
	// the mirror of the Columns of Databse Table at the API-Server.
	// However, they can be freely re-arranged as per the aesthetic.

	public string Username { get; set; } = CrossUtility.CurrentUser();
	public string UserPCName { get; set; } = Environment.MachineName;
	public string UserIP { get; set; } = MacroUtility.IP.ToString();
	public string ScreenshotKeyAWS { get; set; } = string.Empty;		// AWS S3 Key
	public string CurrentApp { get; set; } = string.Empty;				// Currently Executing Application
	public string LogTime { get; set; } = string.Empty;					// Time of the Screenshot-Capture
	public string Version { get; set; } = Configuration.ApplicationsVersion;

	public static ScreenshotEntry Create(string awsRef) => new()
	{
		ScreenshotKeyAWS = awsRef,
		CurrentApp = ExtractProcessName(awsRef),
		LogTime = ExtractTimeStamp(awsRef).ToString("yyyy-MM-dd HH:mm:ss"),
	};

	// Utilities
	// ---------

	public static string Serialize(ScreenshotEntry entry) 
		=> System.Text.Json.JsonSerializer.Serialize(entry, WebAPI.OptionsJSON);

	public static string Serialize(System.Collections.Generic.IEnumerable<ScreenshotEntry> entries) 
		=> System.Text.Json.JsonSerializer.Serialize(entries, WebAPI.OptionsJSON);

	private static string ExtractProcessName(string key)
	{
		var l = key.LastIndexOf(Configuration.Screenshots.ImagesDelimiter, StringComparison.Ordinal) + Configuration.Screenshots.ImagesDelimiter.Length;
		var r = key.LastIndexOf('.');

		return IsValidRange(l, r)
			? key[l..r]
			: Configuration.NotAvailableOrFound;
	}

	private static DateTime ExtractTimeStamp(string key)
	{
		var l = key.LastIndexOf(WebAPI.AWS.BacksSlash) + 1;
		var r = key.IndexOf(Configuration.Screenshots.ImagesDelimiter, StringComparison.Ordinal);

		return IsValidRange(l, r)
			? MacroUtility.DecodeDate(key[l..r])
			: DateTime.Now;
	}

	private static bool IsValidRange(int l, int r) => l < r && l != -1 && r != -1;
}
