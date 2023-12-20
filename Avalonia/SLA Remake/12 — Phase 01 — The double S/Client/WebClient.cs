﻿using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.Net;
using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

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
			username = "SLA Remake - Logger",
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

	// AWS S3 Utility Class
	// --------------------

	public static class AWS
	{
		// Format:
		// Upload(string jpgPath): string (returns the key of the uploaded image)
		// Key Format: <Controls.SLA_GUID_Forever>---<Path.GetFileName(filePath)>

        private const string bucket = "sla-documents";

        private static readonly AmazonS3Client client = new(
            "AKIARYXL6P2LDR5IYXN5",
            "maY4buTCyWbXmqHoRvHz46jSPGtoGz6mA94x4WP+",
            RegionEndpoint.EUWest2
        );

		public static string Upload(string filePath)
		{
            var key = $"{Controls.SLA_GUID_Forever}---{System.IO.Path.GetFileName(filePath)}";
            var req = new PutObjectRequest
			{
                BucketName = bucket,
                Key = key,
                FilePath = filePath,
                ContentType = "image/jpeg"
            };
            var res = client.PutObjectAsync(req).Result;
			return key;
        }

		public static bool DownloadAt(string key, string imgPath)
		{
			var req = new GetObjectRequest
			{
                BucketName = bucket,
                Key = key
            };
			var res = client.GetObjectAsync(req).Result;
			res.WriteResponseStreamToFileAsync(imgPath, false, default).Wait();
			return true;
		}
    }
}