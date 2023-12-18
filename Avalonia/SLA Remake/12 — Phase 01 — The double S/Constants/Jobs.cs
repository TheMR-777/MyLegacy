using System;
using System.Collections.Generic;

namespace SLA_Remake;

public class Jobs
{
	public static readonly List<KeyValuePair<Action, Func<TimeSpan>>> JobsList =
	[
		new(PostmanJob, () => TimeSpan.FromSeconds(17)),
		new(CameramanJob, () => TimeSpan.FromMinutes(new Random().Next(1, 15))),
	];

	private static void PostmanJob()
	{
		if (!Controls.EnableLoggingOnAPI) return;

		if (!WebAPI.VerifyDatabase()) return;
		var entries = Database.GetSavedEntries();

		if (entries.Count == 0) return;
		var success = WebAPI.SendEntries(entries);

		if (!success) return;
		Database.Clear();
	}

	private static void CameramanJob()
	{
		if (!Controls.CaptureScreenshots) return;
		CrossUtility.CaptureAndSaveScreenshot();
	}
}