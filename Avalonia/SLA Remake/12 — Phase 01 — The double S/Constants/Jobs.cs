using System;
using System.Collections.Generic;

namespace SLA_Remake;

public static class Jobs
{
	// This class contains all the required jobs
	// that are to be executed in the background

	private static readonly List<KeyValuePair<Action, Func<TimeSpan>>> Workflows =
	[
		new(PostmanJob, () => TimeSpan.FromSeconds(17)),
		new(CameramanJob, () => TimeSpan.FromMinutes(new Random().Next(1, 15))),
	];

	// Jobs
	// ----

	private static void PostmanJob()
	{
		if (!Controls.EnableLoggingOnAPI) return;

		if (!WebAPI.VerifyDatabase()) return;
		var entries = Database<Models.LogEntry>.GetSavedEntries();

		if (entries.Count == 0) return;
		var success = WebAPI.SendEntries(entries);

		if (!success) return;
		Database<Models.LogEntry>.Clear();
	}

	private static void CameramanJob()
	{
		if (!Controls.CaptureScreenshots) return;
		CrossUtility.CaptureAndSaveScreenshot();
	}

	// Utilities
	// ---------

	public static void Launch() => Workflows.ForEach(RegisterJob);

	private static void RegisterJob(KeyValuePair<Action, Func<TimeSpan>> JobInfo)
	{
		var (Job, GetInterval) = JobInfo;
		System.Threading.Tasks.Task.Run(async () =>
		{
			while (true) 
			{
				try
				{
					await System.Threading.Tasks.Task.Delay(GetInterval());
					Job();
				}
				catch (Exception x)
				{
					WebAPI.RegisterException(x);
				}
			}
		});
	}
}