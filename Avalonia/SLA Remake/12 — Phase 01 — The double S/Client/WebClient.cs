using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
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

    public static void L1_RegisterException(Exception x)
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

        // Body Building
        // -------------

        var body = new
        {
            username = "SLA Remake - Logger",
            avatar_url = "https://i.imgur.com/IvgCM1R.png",
            content = "",
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
                    fields = new List<Dictionary<string, string>>
                    {
                        new()
                        {
                            { "name", "Method Trail" },
                            { "value", $"```{string.Join(" < ", footprints)}```" },
                            { "inline", "true" }
                        },
                        new()
                        {
                            { "name", "Exception/Message" },
                            { "value", $"```{x.Message}```" },
                        },
                        new()
                        {
                            { "name", "Stack-Trace" },
                            { "value", $"```{x.StackTrace}```" },
                        }
                    },
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

        PostDataTo(Controls.DiscordWebhookURL, System.Text.Json.JsonSerializer.Serialize(body));
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
        var entriesFormatted = entries.Select(entry =>
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
            data = entriesFormatted
        };

        return System.Text.Json.JsonSerializer.Serialize(req, new System.Text.Json.JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }
}