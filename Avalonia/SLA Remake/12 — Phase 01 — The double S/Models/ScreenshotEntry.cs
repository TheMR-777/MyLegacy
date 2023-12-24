using System;

namespace SLA_Remake.Models;

public class ScreenshotEntry
{
	// This class contains required information about the screenshot
	// that is to be saved in the Database, after it is posted at S3

	public string Username { get; set; }
	public string UserIP { get; set; }
	public string UserPCName { get; set; }
	public string ScreenshotAWS { get; set; }
	public string CurrentApp { get; set; }
	public string LogTime { get; set; }
	public string AgentVersion { get; set; }

	public static ScreenshotEntry Create(string awsRef) => new()
	{
		Username = CrossUtility.CurrentUser(),
		UserIP = Utility.IP.ToString(),
		UserPCName = Environment.MachineName,
		ScreenshotAWS = awsRef,
		CurrentApp = ExtractProcessName(awsRef),
		LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
		AgentVersion = Configuration.ApplicationVersion
	};

	private static string ExtractProcessName(string key)
	{
        var l = key.LastIndexOf(Configuration.ImagesDelimiter, StringComparison.Ordinal) + Configuration.ImagesDelimiter.Length;
        var r = key.LastIndexOf('.');

		return l > r || l == -1 || r == -1 || r - l < 2
			? "N/A"
            : key[l..r];
    }
}
