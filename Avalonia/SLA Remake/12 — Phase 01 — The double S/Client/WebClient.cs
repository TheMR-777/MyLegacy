using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.IO;
using System;

namespace SLA_Remake;

public static class WebAPI
{
	private const string DiscordWebhookAddressSLA = "https://discord.com/api/webhooks/1172483585698185226/M1oWUKwwl-snXr6sHTeAQoKYQxmg4JVg-tRKkqUZ1gSuYXwsV5Q9DhZj00mMX0_iui6d";
	private static readonly HttpClient _webClient = new()
	{
		BaseAddress = new Uri("https://sla.cash-n-cash.co.uk/api/products/")
	};
	public static readonly System.Text.Json.JsonSerializerOptions OptionsJSON = new()
	{
		DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
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

		return new Version(Configuration.ApplicationsVersion).CompareTo(new Version(version)) >= 0;
	}

	public static bool VerifyDatabase()
	{
		if (!ConnectedToInternet()) return false;
		var status = GetDataFrom(databaseConnect);

		return bool.TryParse(status, out var result) && result;
	}

	public static bool SendEntries(IEnumerable<Models.LogEntry> entries)
	{
		if (!ConnectedToInternet()) return false;

		using var res = PostDataTo(logEntryLogging, Serialize(entries));
		return res.Content.ReadAsStringAsync().Result == "1";
	}

	public static void RegisterException(Exception x, bool verbose = false)
	{
		if (!Configuration.EnableLogOnDiscord) return;
		if (!ConnectedToInternet()) return;

		// Fetching Footprints
		// -------------------

		var footprints = new StackTrace().GetFrames()!
			.Select(frame => frame.GetMethod()!)
			.Select(methodReference => $"> {(verbose ? ".." : methodReference.DeclaringType!)}.{methodReference.Name}({string.Join(", ", methodReference.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray())})");

		// Fetching User Information
		// -------------------------

		var userInf = new[]
		{
			CrossUtility.CurrentUser(deepFetch: true),
			Environment.MachineName,
		};

		var footers = new[]
		{
			// Application Version
			$"v{Configuration.ApplicationsVersion}",

			// System Information
			Environment.Is64BitOperatingSystem ? "x64" : "x86",
			Environment.OSVersion.ToString(),

			// Timestamp
			$"{DateTime.Now:dd-MM-yyyy hh':'mm':'ss tt}",
		};

		// Reformatting Stack-Trace
		// ------------------------

		var newTrace = verbose
			? FormatStackTraceVerbose(x.StackTrace)
			: FormatStackTrace(x.StackTrace);

		// Body Building
		// -------------

		var body = new
		{
			username = Configuration.MyName + " - Logger",
			avatar_url = "https://i.imgur.com/IvgCM1R.png",
			embeds = new List<object>
			{
				new
				{
					author = new
					{
						name = string.Join("  |  ", userInf),
						url = "https://www.google.com",
						icon_url = "https://i.imgur.com/WoNyIIX.png"
					},
					color = 16007990,

					// The embeds are not being used
					// due to the limited characters
					// support, by free-tier Discord

					description = 
					$"**Message of Exception** \n" +
					$"```{x.Message}``` \n" +
					$"**Method Trail** \n" +
					$"```{string.Join('\n', footprints.Take(3))}``` \n" +
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

		using var res = PostDataTo(DiscordWebhookAddressSLA, System.Text.Json.JsonSerializer.Serialize(body, OptionsJSON));
	}

	// Web-Utilities
	// -------------

	public static bool ConnectedToInternet()
	{
		if (!Configuration.EnableTheInternet) return false;

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
			UserName = CrossUtility.CurrentUser() + '~' + Environment.MachineName,
			logDate = now.Date.ToString("dd/MM/yyyy HH:mm:ss") + "~WQoCW/gL8O/+pi0RP2l6xg==",
			LogDateTimeStamp = now.ToString("dd/MM/yyyy HH:mm:ss"),
			version = Configuration.ApplicationsVersion,
			data = formatted
		};

		return System.Text.Json.JsonSerializer.Serialize(req, OptionsJSON);
	}

	private static string FormatStackTrace(string stackTrace) => LowLevel_APIs.StackTraceMarksman().Replace(stackTrace, match => 
		match.Groups["filename"].Success && match.Groups["line"].Success
			? $"| {match.Groups["method"].Value}\n> {match.Groups["filename"].Value}:line {match.Groups["line"].Value}"
			: $"| {match.Groups["method"].Value}");

	private static string FormatStackTraceVerbose(string stackTrace) => LowLevel_APIs.StackTraceMarksman().Replace(stackTrace, match => {
		var line = $"- Method: {match.Groups["method"].Value}";

		if (!match.Groups["filename"].Success) return line;
		line += $"\n\t- File: {match.Groups["filename"].Value}" +
				$"\n\t- Line: {match.Groups["line"].Value}";

		return line;
	});

	// AWS-S3 Utility Class
	// --------------------

	public static class AWS
	{
		public const char BacksSlash = '/';
		private const string _bucket = "sla-documents";
		private static readonly string _prefix = $"{Configuration.MyName}{BacksSlash}{CrossUtility.CurrentUser()}@{Environment.MachineName}{BacksSlash}";
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
			var keys = new System.Collections.Concurrent.ConcurrentBag<string>();
			var path = new DirectoryInfo(imgFolder);
			if (!path.Exists) return [];

			path.GetFiles('*' + Configuration.ImagesExtension).AsParallel().WithDegreeOfParallelism(Configuration.MaxTransmissionThreads).ForAll(file =>
			{
				// This function has its own try-catch block
				// as, if in-case of any exception at file's
				// deletion, it won't disrupt the whole Job.
				// That would cause the loss of created keys

				try
				{
					var key = Upload(file.FullName);
					if (string.IsNullOrEmpty(key)) return;
					file.Delete();
					keys.Add(key);
				}
				catch (Exception x)
				{
					RegisterException(x);
				}
			});

			return [.. keys];
		}

		public static bool DownloadScreenshotsTo(IEnumerable<string> keys, string targetFolder)
		{
			var success = true;
			var catalog = new DirectoryInfo(targetFolder);
			if (!catalog.Exists) return false;

			keys.AsParallel().WithDegreeOfParallelism(Configuration.MaxTransmissionThreads).ForAll(key =>
				success &= DownloadAt(key, Path.Combine(catalog.FullName, key))
			);

			return success;
		}

		// Main Functions
		// --------------
		// The following methods are
		// key-methods of this class
		// containing all the logics

		private static string Upload(string filePath)
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

		private static bool DownloadAt(string key, string filePath)
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