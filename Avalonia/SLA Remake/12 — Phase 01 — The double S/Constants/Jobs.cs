using SLA_Remake.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLA_Remake;

public static class Jobs
{
	// This class contains all the required jobs
	// that are to be executed in the background

	private static readonly Random _random = new();
	private static readonly List<Tuple<Action, Func<TimeSpan>>> Workflows =
	[
		new(PostmanJob, () => TimeSpan.FromSeconds(17)),
		//new(CameramanJob, () => TimeSpan.FromSeconds(_random.Next(5, 15))),
		new(CameramanJob, () => TimeSpan.FromMinutes(_random.Next(5, 15))),
	];

	// Jobs
	// ----

	private static void PostmanJob()
	{
		// Responsibility:
		// ---------------
		// Postman's Job is to send all the saved logs to the API
		// and then, clear the respective local SQLite3 Database.

		if (!Configuration.EnableLoggingOnAPI) return;

		if (!WebAPI.VerifyDatabase()) return;
		var log_entries = Database<LogEntry>.GetSavedEntries();

		if (log_entries.Count == 0) return;
		var success = WebAPI.SendEntries(log_entries);

		if (!success) return;
		Database<LogEntry>.Clear();
	}

	private static void CameramanJob()
	{
		// Responsibility:
		// ---------------
		// Cameraman's Job is to capture screenshots and save them locally
		// and then, try to upload all the screenshots to AWS S3, and make
		// keys. Then save the keys obtained in the Local SQLite3 Database

		CrossUtility.CaptureAndSaveScreenshot();

		if (!Configuration.EnableCacheLogging) return;
		if (!WebAPI.ConnectedToInternet()) return;

		var keys = WebAPI.AWS.UploadScreenshotsFrom(Configuration.ScreenshotsFolder);
		if (keys.Count == 0) return;

		var log = keys.Select(ScreenshotEntry.Create);
		var res = Database<ScreenshotEntry>.Save(log);
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