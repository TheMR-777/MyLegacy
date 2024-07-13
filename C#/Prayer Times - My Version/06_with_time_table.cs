using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace MyPlayground;

internal class Program
{
    private static void Main()
    {
        var today = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

        var apiRes = new HttpClient().GetAsync($"https://api.aladhan.com/v1/timingsByCity/{today}?city=Jhelum&state=Punjab&country=Pakistan&method=8&school=1").Result;
        if (!apiRes.IsSuccessStatusCode) return;
        var prayers = new[] { "Fajr", "Dhuhr", "Asr", "Maghrib", "Isha" };
        var timings = JObject.Parse(apiRes.Content.ReadAsStringAsync().Result)["data"]?["timings"];

        var moment = DateTime.Now.TimeOfDay;
        var prayer = 0;

        for (var i = 0; i < prayers.Length; i++)
        {
            var namazTime = DateTime.ParseExact(timings?[prayers[i]]?.ToString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            if (moment < namazTime) break;
            prayer = i;
        }

        var current = prayers[prayer];
        var nextOne = prayers[(prayer + 1) % prayers.Length];

        var now = DateTime.ParseExact(timings?[current]?.ToString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt", CultureInfo.InvariantCulture);
        var end = DateTime.ParseExact(timings?[nextOne]?.ToString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt", CultureInfo.InvariantCulture);

        if (current == "Dhuhr")
            current = "Zuhr";

        var timeTable = string.Join(" | ", prayers.Select(p => DateTime.ParseExact(timings?[p]?.ToString() ?? string.Empty, "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt", CultureInfo.InvariantCulture)));
        timeTable = $"```\n| Fajr     | Zuhr     | Asr      | Maghrib  | Isha     |\n| {timeTable} |\n```";

        var formatted = $"Break for **{current}**\n_Timing of {current}:_ [ _starts:_ **{now}** - _ends:_ **{end}** ]\n{timeTable}";

        SetClipboard(formatted);
        Console.WriteLine(formatted);
    }

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
