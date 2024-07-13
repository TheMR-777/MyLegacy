using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        var apiRes = new HttpClient().GetAsync("https://api.aladhan.com/v1/timingsByCity/20-10-2023?city=Jhelum&state=Punjab&country=Pakistan&method=1&school=1").Result;

        if (apiRes.IsSuccessStatusCode)
        {
            var prayers = new[] { "Fajr", "Dhuhr", "Asr", "Maghrib", "Isha" };
            var timings = JObject.Parse(apiRes.Content.ReadAsStringAsync().Result)["data"]?["timings"];

            var moment = DateTime.Now.TimeOfDay;
            int prayer = 0;

            for (int i = 0; i < prayers.Length; i++)
            {
                var namazTime = DateTime.ParseExact(timings[prayers[i]].ToString(), "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

                if (moment < namazTime)
                    break;

                prayer = i;
            }

            var current = prayers[prayer];
            var nextOne = prayers[(prayer + 1) % prayers.Length];

            var now = DateTime.ParseExact(timings[current].ToString(), "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt", CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(timings[nextOne].ToString(), "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt", CultureInfo.InvariantCulture);

            var formatted = $"*{current}* Break\n_{current} Timing:_ [ _Start:_ *{now}* - _End:_ *{end}* ]";

            SetClipboard(formatted);
            Console.WriteLine(formatted);
        }
    }

    public static void SetClipboard(string value)
    {
        var clipboardExecutable = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                FileName = @"clip",
            }
        };
        clipboardExecutable.Start();
        clipboardExecutable.StandardInput.Write(value);
        clipboardExecutable.StandardInput.Close();
    }
}