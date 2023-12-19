using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.Net;
using System.Text.RegularExpressions;

namespace SLA_Remake;

public static partial class WebAPI
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

		var newTrace = StackTraceRegex().Replace(x.StackTrace ?? " --- ", "|");

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

    [GeneratedRegex(@"^\s*at", RegexOptions.Multiline)]
    private static partial Regex StackTraceRegex();

	private static readonly System.Text.Json.JsonSerializerOptions options = new() 
	{ 
		DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
	};
}

public static class AWSAPI
{
	private const string bucket = "sla-ucket";

	private static readonly AmazonS3Client client = new(
		"AKIARYXL6P2LMFXNG26T",
		"p51H8gQE10c02hQfXmrqGQlwe3oS7AyhBzQfyUYG",
		RegionEndpoint.EUWest2
	);

	public static string Upload(string filePath)
	{
		var fileName = System.IO.Path.GetFileName(filePath);
		var memberFilePath = "";
		try
		{
			var putRequest = new PutObjectRequest
			{
				BucketName = bucket,
				Key = $"memberfilesTest/{fileName}",
				FilePath = filePath,
				ContentType = "image/jpeg"
			};

			var response = client.PutObjectAsync(putRequest).Result;

			if (response is { HttpStatusCode: HttpStatusCode.OK })
			{
				memberFilePath = $"memberfilesTest/{fileName}";
			}
		}
		catch (AmazonS3Exception amazonS3Exception)
		{
			if (amazonS3Exception.ErrorCode is "InvalidAccessKeyId" or "InvalidSecurity")
			{
				throw new Exception("Check the provided AWS Credentials.");
			}
			throw new Exception("Error occurred: " + amazonS3Exception.Message);
		}
		catch (Exception e)
		{
			throw new Exception("Unknown error occurred: " + e.Message);
		}
		
		return $"https://{bucket}.s3.{client.Config.RegionEndpoint.SystemName}.amazonaws.com/{memberFilePath}";
	}

	public static string UploadWithTransfer(string imgPath)
	{
		//try
		//{
			var imgName = System.IO.Path.GetFileName(imgPath);
			var utility = new TransferUtility(client);
			var request = new TransferUtilityUploadRequest
			{
				BucketName = bucket,
				Key = $"memberfilesTest/{imgName}",
				FilePath = imgPath,
				ContentType = "image/jpeg"
			};
			utility.Upload(request);
			return $"https://{bucket}.s3.{client.Config.RegionEndpoint.SystemName}.amazonaws.com/memberfilesTest/{imgName}";
		//}
		//catch (Exception x)
		//{
		//	return x.Message;
		//}
	}
}