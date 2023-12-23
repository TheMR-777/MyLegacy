using SLA_Remake.Models;
using System;
using System.Collections.Generic;

namespace SLA_Remake;

public static class Jobs
{
	// This class contains all the required jobs
	// that are to be executed in the background

	private static readonly Random _random = new();
	private static readonly List<Tuple<Action, Func<TimeSpan>>> Workflows =
	[
		new(PostmanJob, () => TimeSpan.FromSeconds(17)),
		new(CameramanJob, () => TimeSpan.FromMinutes(_random.Next(1, 15))),
	];

	// Jobs
	// ----

	private static void PostmanJob()
	{
		if (!Controls.EnableLoggingOnAPI) return;

		if (!WebAPI.VerifyDatabase()) return;
		var log_entries = Database<LogEntry>.GetSavedEntries();

		if (log_entries.Count == 0) return;
		var success = WebAPI.SendEntries(log_entries);

		if (!success) return;
		Database<LogEntry>.Clear();
	}

	private static void CameramanJob()
	{
		if (!Controls.CaptureScreenshots) return;
		CrossUtility.CaptureAndSaveScreenshot();

		if (!WebAPI.ConnectedToInternet()) return;
		Utility.CreateEntriesFromSavedScreenshots();
	}

	// Utilities
	// ---------

	public static void Launch() => Workflows.ForEach(RegisterJob);

	private static void RegisterJob(Tuple<Action, Func<TimeSpan>> JobInfo)
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