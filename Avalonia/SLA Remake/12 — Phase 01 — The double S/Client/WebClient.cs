using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.IO;
using System;

namespace SLA_Remake;

public static class WebAPI
{
	private static readonly HttpClient _webClient = new()
	{
		BaseAddress = new Uri("https://sla.cash-n-cash.co.uk/api/products/")
	};

	// The Endpoints
	// -------------

	private const string deployedVersion = "GetCurrentSLAVersion";
	private const string databaseConnect = "GetCheckDBConnection";
	private const string logEntryLogging = "GetSLALogEntries";

	// Main Methods
	// ------------

	public static bool VerifyVersion()
	{
		if (!ConnectedToInternet()) return true;
		var version = GetDataFrom(deployedVersion);

		return new Version(Controls.ApplicationVersion).CompareTo(new Version(version)) >= 0;
	}

	public static bool VerifyDatabase()
	{
		if (!ConnectedToInternet()) return false;
		var status = GetDataFrom(databaseConnect);

		try
		{
			return bool.Parse(status);
		}
		catch
		{
			return false;
		}
	}

	public static bool SendEntries(List<Models.LogEntry> entries)
	{
		if (!ConnectedToInternet()) return false;

		using var res = PostDataTo(logEntryLogging, Serialize(entries));
		return res.Content.ReadAsStringAsync().Result == "1";
	}

	public static void RegisterException(Exception x)
	{
		if (!Controls.EnableLogOnDiscord) return;

		// Fetching Footprints
		// -------------------

		var footprints = new StackTrace().GetFrames()!
			.Select(frame => frame.GetMethod()!)
			.Select(methodReference => $"{methodReference.DeclaringType!}.{methodReference.Name}({string.Join(", ", methodReference.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray())})").ToList();

		// Fetching User Information
		// -------------------------

		var userInf = new[]
		{
			CrossUtility.GetCurrentUser(deepFetch: true),
			Environment.MachineName,
		};

		var footers = new[]
		{
			// Application Version
			$"v{Controls.ApplicationVersion}",

			// Timestamp
			$"{DateTime.Now:dd-MM-yyyy hh':'mm':'ss tt}"
		};

		// Re-Formatting Stack-Trace
		// -------------------------

		var newTrace = LowLevel_APIs.StackTraceRegex().Replace(x.StackTrace ?? " --- ", "|");

		// Body Building
		// -------------

		var body = new
		{
			username = Controls.MyName + " - Logger",
			avatar_url = "https://i.imgur.com/IvgCM1R.png",
			embeds = new List<object>
			{
				new
				{
					author = new
					{
						name = string.Join("  |  ", userInf),
						url = "https://www.google.com",
						icon_url = "https://i.imgur.com/xCvzudW.png"
					},
					color = 16007990,
					thumbnail = new
					{
						url = "https://i.imgur.com/IvgCM1R.jpg"
					},

					// The embeds are not being used
					// due to the limited characters
					// support, by free-tier Discord

					description = 
					$"**Message of Exception** \n" +
					$"```{x.Message}``` \n" +
					$"**Method Trail** \n" +
					$"```{string.Join(" < ", footprints)}``` \n" +
					$"**Stack-Trace** \n" +
					$"```{newTrace}```",
					fields = new List<Dictionary<string, string>> { },
					footer = new
					{
						text = string.Join("   |   ", footers),
						icon_url = "https://i.imgur.com/0jqoy6w.jpg"
					}
				}
			}
		};

		// Sending the Request
		// -------------------

		using var res = PostDataTo(Controls.DiscordWebhookURL, System.Text.Json.JsonSerializer.Serialize(body, options));
	}

	// Web-Utilities
	// -------------

	public static bool ConnectedToInternet()
	{
		if (!Controls.EnableTheInternet) return false;

		var connected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
		if (!connected) return false;

		// GetIsNetworkAvailable is Fast but Unreliable for 'true'
		// So, we go for the slower method, if that returns 'true'

		try
		{
			using var web = new HttpClient();
			using var res = web.GetAsync("https://google.com/generate_204").Result;
			connected = res.IsSuccessStatusCode;
		}
		catch
		{
			connected = false;
		}
		return connected;
	}

	private static string GetDataFrom(string endPoint)
	{
		using var res = _webClient.GetAsync(endPoint).Result;
		return res.Content.ReadAsStringAsync().Result.Trim('"');
	}

	private static HttpResponseMessage PostDataTo(string endPoint, string data)
	{
		using var req = new HttpRequestMessage(HttpMethod.Post, endPoint)
		{
			Content = new StringContent(
				data,
				new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
			)
		};
		return _webClient.SendAsync(req).Result;
	}

	private static string Serialize(IEnumerable<Models.LogEntry> entries)
	{
		// The Serialization is being done
		// as according to the legacy SLA,
		// to maintain the API-consistency

		var formatted = entries.Select(entry =>
		{
			var properties = entry.GetType().GetProperties();
			var values = properties.Select(property => property.GetValue(entry, null));
			return string.Join("|", values);
		});

		var now = DateTime.Now;
		var req = new
		{
			UserName = CrossUtility.GetCurrentUser() + '~' + Environment.MachineName,
			logDate = now.Date.ToString("dd/MM/yyyy HH:mm:ss") + "~WQoCW/gL8O/+pi0RP2l6xg==",
			LogDateTimeStamp = now.ToString("dd/MM/yyyy HH:mm:ss"),
			version = Controls.ApplicationVersion,
			data = formatted
		};

		return System.Text.Json.JsonSerializer.Serialize(req, options);
	}

	private static readonly System.Text.Json.JsonSerializerOptions options = new() 
	{ 
		DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
	};

	// AWS-S3 Utility Class
	// --------------------

	public static class AWS
	{
		private const string _bucket = "sla-documents";
		private static readonly string _prefix = $"{Controls.MyName}/{CrossUtility.GetCurrentUser()}@{Environment.MachineName}/";
		private static readonly Amazon.S3.Transfer.TransferUtility _awsClient = new(
			"AKIARYXL6P2LDR5IYXN5",
			"maY4buTCyWbXmqHoRvHz46jSPGtoGz6mA94x4WP+",
			Amazon.RegionEndpoint.EUWest2
		);

		// Wrapper Functions
		// -----------------
		// The following methods are the
		// utility methods of this class
		// which execute the key-methods
		// in parallel & thread-safe way

		public static List<string> UploadScreenshotsFrom(string imgFolder)
		{
			var keys = new List<string>();
			var path = new DirectoryInfo(imgFolder);
			if (!path.Exists) return [];

			path.GetFiles('*' + Controls.ImagesExtension).AsParallel().WithDegreeOfParallelism(Controls.MaxTransmissionThreads).ForAll(file =>
			{
				var key = Upload(file.FullName);
				if (string.IsNullOrEmpty(key)) return;
				keys.Add(key);
				file.Delete();
			});

			return keys;
		}

		public static bool DownloadScreenshotsTo(List<string> keys, string targetFolder)
		{
			var success = true;
			var catalog = new DirectoryInfo(targetFolder);
			if (!catalog.Exists) return false;

			keys.AsParallel().WithDegreeOfParallelism(Controls.MaxTransmissionThreads).ForAll(key =>
				success &= DownloadAt(key, Path.Combine(catalog.FullName, key))
			);

			return success;
		}

		// Main Functions
		// --------------
		// The following methods are
		// key-methods of this class
		// containing all the logics

		public static string Upload(string filePath)
		{
			try
			{
				var key = _prefix + Path.GetFileName(filePath);
				_awsClient.Upload(filePath, _bucket, key);
				return key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public static bool DownloadAt(string key, string filePath)
		{
			try
			{
				_awsClient.Download(filePath, _bucket, key);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}