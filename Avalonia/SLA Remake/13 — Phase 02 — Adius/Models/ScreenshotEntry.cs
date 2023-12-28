using System;

namespace SLA_Remake.Models;

public class ScreenshotEntry
{
	// This class contains required information about the screenshot
	// that is to be saved in the Database, after it is posted at S3

	public string Username { get; set; }
	public string UserIP { get; set; }
	public string UserPCName { get; set; }
	public string ScreenshotKeyAWS { get; set; }	// AWS S3 Key
	public string CurrentApp { get; set; }			// Currently Executing Application
	public string LogTime { get; set; }				// Time of the Screenshot-Capture
	public string Version { get; set; }				// Agent Version

	public static ScreenshotEntry Create(string awsRef) => new()
	{
		Username = CrossUtility.CurrentUser(),
		UserIP = MacroUtility.IP.ToString(),
		UserPCName = Environment.MachineName,
		ScreenshotKeyAWS = awsRef,
		CurrentApp = ExtractProcessName(awsRef),
		LogTime = ExtractTimeStamp(awsRef).ToString("yyyy-MM-dd HH:mm:ss"),
		Version = Configuration.ApplicationsVersion
	};

	// Utilities
	// ---------

	public string Serialize() => System.Text.Json.JsonSerializer.Serialize(this, WebAPI.OptionsJSON);

	private static string ExtractProcessName(string key)
	{
		var l = key.LastIndexOf(Configuration.Screenshots.ImagesDelimiter, StringComparison.Ordinal) + Configuration.Screenshots.ImagesDelimiter.Length;
		var r = key.LastIndexOf('.');

		return IsValidRange(l, r)
			? key[l..r]
			: "N/A";
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
