using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        var apiRes = new HttpClient().GetAsync("http://api.aladhan.com/v1/timingsByCity?city=Jhelum&country=Pakistan&method=8").Result;

        if (apiRes.IsSuccessStatusCode)
        {
            var result = apiRes.Content.ReadAsStringAsync().Result;

            var timings = JObject.Parse(result)["data"]?["timings"];

            var prayers = new[] { "Fajr", "Zuhr", "Asr", "Maghrib", "Isha" };

            int i = DateTime.Now.Hour < 12 ? 
                0 : (DateTime.Now.Hour < 15 ? 
                1 : (DateTime.Now.Hour < 18 ? 
                2 : (DateTime.Now.Hour < 20 ? 
                3 : 4)));

            var current = prayers[i];
            var nextOne = prayers[(i + 1) % 5];

            var now = DateTime.ParseExact(timings[current].ToString(), "HH:mm", CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(timings[nextOne].ToString(), "HH:mm", CultureInfo.InvariantCulture);

            var now_f = now.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            var end_f = end.ToString("hh:mm tt", CultureInfo.InvariantCulture);

            var clipboardText = $"{current} — *{now_f}* _to_ *{end_f}*";

            SetClipboard(clipboardText);
            Console.WriteLine(clipboardText);
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