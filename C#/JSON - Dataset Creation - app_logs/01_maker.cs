https://github.com/TheMR-777/MyLegacy/tree/main/C%23/JSON%20-%20Dataset%20Creationusing System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Playground;

public class Entry
{
    public string current_app { get; set; } = string.Empty;
    public string app_title { get; set; } = string.Empty;
    public string registery_date { get; set; } = string.Empty;
}

public static class Program
{
    private static string GenerateJsonDataset(int numberOfEntries, IReadOnlyCollection<string> appNames, IReadOnlyCollection<string> appTitles, DateTime startDate, DateTime endDate)
    {
        var entries = Enumerable.Range(0, numberOfEntries)
            .Select(i => new Entry
            {
                current_app = appNames.ElementAt(Random.Shared.Next(appNames.Count)),
                app_title = appTitles.ElementAt(Random.Shared.Next(appTitles.Count)),
                registery_date = $"{startDate.AddDays(Random.Shared.Next((int)(endDate - startDate).TotalDays)):yyyy-MM-dd}"
            })
            .ToList();

        var jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        var body = new
        {
            system_user = "ASC@THEMR-ASC",
            user_ip = "...",
            sla_version = "3.0.0.00",
            entries
        };

        var json = JsonConvert.SerializeObject(body, jsonSettings);
        return json;
    }

    private static void Main()
    {
        var names = new List<string> 
        {
            "vs-code"
        };
        var heads = new List<string> 
        { 
            "TheMR"
        };
        var entry = GenerateJsonDataset(100000, names, heads, DateTime.Parse("2023-03-01"), DateTime.Parse("2024-05-01"));
        // Console.WriteLine(entry);
        File.WriteAllText("my_dataset.json", entry);
    }
}