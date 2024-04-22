using System.Text.Json;

public class JsonDataGenerator
{
    public class Payload
    {
        public string system_user { get; set; } = string.Empty;
        public string user_ip { get; set; } = string.Empty;
        public string sla_version { get; set; } = string.Empty;
        public List<Entry> entries { get; set; } = [];
    }

    public static string GenerateJsonData(Payload payload, int n, List<string> details, DateTime start, DateTime end, double skew = 0.5)
    {
        for (var i = 0; i < n; i++)
        {
            // Generate a random GUID
            var guid = Guid.NewGuid().ToString();

            // Generate a logout entry
            var logoutEpoch = GenerateRandomTime(start, end.AddHours(-10));
            var logoutEntry = new Entry
            {
                reason_id = 0,
                reason_detail = string.Empty,
                log_timestamp = logoutEpoch,
                log_guid = guid
            };
            payload.entries.Add(logoutEntry);

            // Generate a login entry
            var reasonId = Random.Shared.Next(1, 20);
            var myDetail = reasonId is >= 11 and <= 19 ? details[Random.Shared.Next(details.Count)] : string.Empty;
            var loginEntry = new Entry
            {
                reason_id = reasonId,
                reason_detail = myDetail,
                log_timestamp = GenerateRandomTime(logoutEpoch.AddMinutes(5), logoutEpoch.AddHours(1), skew),
                log_guid = guid
            };
            payload.entries.Add(loginEntry);
        }

        var jsonData = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        return jsonData;
    }

    private static DateTime GenerateRandomTime(DateTime start, DateTime end, double skew = 0.5)
    {
        var ticks = (end - start).Ticks;
        var value = Math.Pow(Random.Shared.NextDouble(), skew);
        var index = (long)(ticks * value);
        return start.AddTicks(index);
    }

    public class Entry
    {
        public int reason_id { get; set; } = 0;
        public string reason_detail { get; set; } = string.Empty;
        public DateTime log_timestamp { get; set; } = DateTime.MinValue;
        public string log_guid { get; set; } = string.Empty;
    }
}

class Program
{
    private static void Main()
    {
        var packs = new JsonDataGenerator.Payload
        {
            system_user = "IK@from-ASC",
            user_ip = "192.168.0.777",
            sla_version = "3.0.0.00",
        };
        var texts = new List<string> 
        {
            "Not your concern man", 
            "Im Batman",
            "Im Ironman",
            "Im Spiderman",
            "Im Superman",
            "You cant see me",
            "Im invisible",
            "Im a ghost",
            "Im a ninja",
            "Why so serious?",
            "Im a joker",
            "Its about sending a message",
            "Im a legend",
            "Why do we fall?",
            "So, you think darkness is your ally?",
            "Roger that",
            "Im a soldier",
            "Target locked",
            "Im a sniper",
            "Target Acquired, Box 2-4-9",
            "Im a pilot",
            "Hey there, I am using WhatsApp",
            "Im a hacker",
            "404 - Reason not found",
            "403 - Access Denied, but why?",
            "500 - Internal Server Error, seriously?",
            "Gotta go fast",
            "Man of culture",
            "Dont worry, Im a doctor",
            "Trust me, Im an engineer",
            "Hey VSauce, Michael here",
            "Hello World",
            "Goodbye World",
            "Nice to meet you",
            "Me: 00t. I have skills, Skills that make me a nightmare for people like you",
            "Me: 01t. If you let my daughter go now, thatll be the end of it",
            "Me: 02t. I will not look for you, I will not pursue you",
            "Me: 03t. But if you dont, I will look for you, I will find you",
            "Me: 04t. And I will ... you know the rest",
            "He: 05t. Good Luck",
            "Me: 01c. Victory can not be achieved without sacrifice, Mason",
            "Me: 02c. We Russians know this better than anyone",
            "Me: 03c. The Germans will not be here today if we had not sacrificed",
            "Me: 04c. Their blood has watered the soil of our motherland",
            "Me: 05c. The sacrifice is not yet over, but I am sure",
            "Me: 06c. That the victory will be ours",
            "Me: 07c. We will not be defeated",
            "Me: 08c. We will not be broken",
            "Me: 09c. We will not be conquered",
            "Me: 10c. We will rise",
            "Me: 11c. We will fight",
            "Me: 12c. We will win",
            "Me: 13c. We will be free, or die trying",
            "01bo1. Every Journey begins with a single step",
            "02bo1. This is Step 1: Secure the keys",
            "03bo1. Now we take Vorkuta!",
            "04bo1. What is Step 2? : Ascend from darkness",
            "05bo1. Step 3? : Rain fire",
            "06bo1. Step 4? : Unleash the horde",
            "07bo1. Step 5? : Skewer the winged beast",
            "08bo1. Step 6? : Wield a fist of iron",
            "09bo1. Step 7? : Raise Hell",
            "10bo1. Step 8? : Freedom",
            "11bo1. Allow me to introduce - Sargei Kozin: The monster of Magadan",
            "12bo1. Glad you are a friend, Sargei",
            "13bo1. Reznov, are you sure we can trust this Amarican?",
            "14bo1. With my life, he and us are not so different",
            "15bo1. We are all soldiers without an army",
            "16bo1. Betrayed, forgotten, abandoned",
            "17bo1. In Vorkuta, we are all brothers",
        };
        var start = DateTime.Now.AddMonths(-1);
        var cease = DateTime.Now.AddMonths(1);
        var skews = Random.Shared.NextDouble();

        // Skew Guide:
        // 0.0 - 0.5: Longer Breaks
        // 0.5 - 1.0: Shorter Breaks

        var jsonData = JsonDataGenerator.GenerateJsonData(packs, 1000, texts, start, cease, skews);
        Console.WriteLine("Skew: " + skews);
        File.WriteAllText("my_dataset_sla-logs.json", jsonData);
        SetClipboard(jsonData);
    }

    private static void SetClipboard(string value)
    {
        var clipboardExecutable = new System.Diagnostics.Process
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
