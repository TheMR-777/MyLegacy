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
		Username = CrossUtility.GetCurrentUser(),
		UserIP = Utility.GetIP().ToString(),
		UserPCName = Environment.MachineName,
		ScreenshotAWS = awsRef,
		CurrentApp = "XYZ",
		LogTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
		AgentVersion = Controls.ApplicationVersion
	};
}
