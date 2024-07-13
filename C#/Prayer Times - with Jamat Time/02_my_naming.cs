using System.Globalization;
using System.Text.Json;

namespace MyPlayground
{
	internal class Program
	{
		const int pad = 8;

		private static void Main()
		{
			var today = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

			var apiRes = new HttpClient().GetAsync($"https://api.aladhan.com/v1/timingsByCity/{today}?city=Jhelum&state=Punjab&country=Pakistan&method=1&school=1").Result;
			if (!apiRes.IsSuccessStatusCode) return;
			var prayers = new[] { "Fajr", "Dhuhr", "Asr", "Maghrib", "Isha" };
			var timings = JsonDocument.Parse(apiRes.Content.ReadAsStringAsync().Result).RootElement.GetProperty("data").GetProperty("timings");

			var mdTable = CreateTimeTable(prayers, timings);
			var headers = $"**Timetable of Prayers — {DateTime.Now:dd/MM/yyyy}**";
			var aligned = $"{headers}\n{mdTable}";

			Console.WriteLine(aligned);
			SetClipboard(aligned);
		}

		private static TimeSpan ParseTime(JsonElement timings, string prayer)
			=> DateTime.ParseExact(timings.GetProperty(prayer).GetString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

		private static string FormatTime(TimeSpan time)
			=> DateTime.Today.Add(time).ToString("hh:mm tt", CultureInfo.InvariantCulture);

		private static string CreateTimeTable(string[] prayers, JsonElement timings)
		{
			var head = "Namaz: | " + string.Join(" | ", prayers.Select(p => ConvertZuhr(p).PadRight(pad))) + " |";
			var row1 = "Start: | " + string.Join(" | ", prayers.Select(p => FormatTime(ParseTime(timings, p)))) + " |";
			var row2 = "Jamat: | " + string.Join(" | ", prayers.Select(p => GetJamatTime(ParseTime(timings, p), p))) + " |";

			return $"```\n{head}\n{row1}\n{row2}\n```";
		}

		private static string GetJamatTime(TimeSpan time, string prayer)
		{
			switch (prayer)
			{
				case "Fajr":
					return string.Empty.PadRight(pad);
				case "Dhuhr":
					return "01:30 PM";
			}

			var delay = prayer == "Maghrib" ? 10 : 15;
			var jamatTime = time.Add(TimeSpan.FromMinutes(delay));

			// Round up to the next quarter-hour
			var minutes = jamatTime.Minutes;
			var quarter = minutes / 15 * 15;
			if (minutes % 15 != 0) quarter += 15;
			jamatTime = new TimeSpan(jamatTime.Hours, quarter, 0);

			return FormatTime(jamatTime);
		}

		private static string ConvertZuhr(string current)
			=> current == "Dhuhr" ? "Zuhr" : current;

		private static void SetClipboard(string value)
		{
			var clipboard = new System.Diagnostics.Process
			{
				StartInfo = new()
				{
					RedirectStandardInput = true,
					FileName = "clip",
				}
			};
			clipboard.Start();
			clipboard.StandardInput.Write(value);
			clipboard.StandardInput.Close();
		}
	}
}
