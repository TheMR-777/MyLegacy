using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace MyPlayground;

internal class Program
{
	private static void Main()
	{
		var today = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

		var apiRes = new HttpClient().GetAsync($"https://api.aladhan.com/v1/timingsByCity/{today}?city=Jhelum&state=Punjab&country=Pakistan&method=8&school=1").Result;
		if (!apiRes.IsSuccessStatusCode) return;
		var prayers = new[] { "Fajr", "Dhuhr", "Asr", "Maghrib", "Isha" };
		var timings = JsonDocument.Parse(apiRes.Content.ReadAsStringAsync().Result).RootElement.GetProperty("data").GetProperty("timings");

		var moment = DateTime.Now.TimeOfDay;
		var prayer = 0;

		for (var i = 0; i < prayers.Length; i++)
		{
			var namazTime = ParseTime(timings, prayers[i]);
			if (moment < namazTime) break;
			prayer = i;
		}

		var current = prayers[prayer];
		var nextOne = prayers[(prayer + 1) % prayers.Length];

		var now = PrayerTime(timings, current);
		var end = PrayerTime(timings, nextOne);

		current = ConvertZuhr(current);

		var formatted = $"Break for **{current}**\n**{current}** Time: [ _starts:_ **{now}** - _ends:_ **{end}** ]\n{CreateTimeTable(prayers, timings)}";

		SetClipboard(formatted);
		Console.WriteLine(formatted);
	}

	private static string PrayerTime(JsonElement timings, string prayer)
		=> FormatTime(ParseTime(timings, prayer));

	private static TimeSpan ParseTime(JsonElement timings, string prayer)
		=> DateTime.ParseExact(timings.GetProperty(prayer).GetString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

	private static string FormatTime(TimeSpan time) 
		=> DateTime.Today.Add(time).ToString("hh:mm tt", CultureInfo.InvariantCulture);

	private static string CreateTimeTable(string[] prayers, JsonElement timings)
	{
		var header = string.Join(" | ", prayers.Select(p => ConvertZuhr(p).PadRight(8)));
		var row = string.Join(" | ", prayers.Select(p => FormatTime(ParseTime(timings, p))));
		return $"```\n| {header} |\n| {row} |\n```";
	}

	private static string ConvertZuhr(string current)
		=> current == "Dhuhr" ? "Zuhr" : current;

	private static void SetClipboard(string value)
	{
		var clipboardExecutable = new Process
		{
			StartInfo = new()
			{
				RedirectStandardInput = true,
				FileName = "clip",
			}
		};
		clipboardExecutable.Start();
		clipboardExecutable.StandardInput.Write(value);
		clipboardExecutable.StandardInput.Close();
	}
}
