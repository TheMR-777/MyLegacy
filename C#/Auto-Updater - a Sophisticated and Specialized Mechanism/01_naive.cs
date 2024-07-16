using System.Diagnostics;
using System.IO.Compression;

namespace MyEvolver;

static class Evolver
{
    public static void Main(string[] arguments)
    {
        // Initialization
        // --------------

        var parent = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
        const string myName = "SLA-Remake";
        const string target = $"{myName} - Windows";
        const string sqlite = $"{myName}.sqlite";
        const string exeSLA = $"{myName}.exe";

        // Arguments Check
        // ---------------

        if (arguments.Length != 1)
        {
            Console.WriteLine("The path of zip-file is not provided");
            Console.WriteLine("Usage: evolver <path-to-zip-archive>");
            return;
        }

        // Path Validation
        // ---------------

        var path = arguments.FirstOrDefault() ?? string.Empty;
        if (!File.Exists(path))
        {
            Console.WriteLine("The provided path is not valid");
            Console.WriteLine("Provided Path: " + path);
            return;
        }

        // Terminate the Client App
        // -------------------------

        var processes = Process.GetProcesses();
        processes.Where(p => p.ProcessName.Contains("SLA-Remake", StringComparison.OrdinalIgnoreCase)).ToList().ForEach(p => p.Kill());

        // Backup the Current Version
        // ---------------------------

        var backup = Path.Combine(parent, "Backups", $"{target} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}");
        Directory.CreateDirectory(backup);
        foreach (var file in Directory.GetFiles(Path.Combine(parent, target), "*", SearchOption.AllDirectories))
        {
            var relative = file.Replace(Path.Combine(parent, target), string.Empty).TrimStart(Path.DirectorySeparatorChar);
            var destination = Path.Combine(backup, relative);
            Copy(file, destination);
        }

        // Unzip the Archive
        // -----------------

        var temp = Path.Combine(parent, "Temp");
        ZipFile.ExtractToDirectory(path, temp, true);

        // Verify the Unzipped Files
        // -------------------------
        // TODO: Error can be Recovered, with a Sophisticated Mechanism

        if (!File.Exists(Path.Combine(temp, target, exeSLA)) || !File.Exists(Path.Combine(temp, target, sqlite)))
        {
            Console.WriteLine("The provided zip-file is not valid");
            Console.WriteLine("Provided Path: " + path);
            Console.WriteLine();
            Console.WriteLine("The folder structure must be in the following pattern");
            Console.WriteLine("SLA-Remake - Windows");
            Console.WriteLine("├── SLA-Remake.exe (executable)");
            Console.WriteLine("├── SLA-Remake.sqlite (cache)");
            Console.WriteLine("└── ...");
            return;
        }

        // Replace the Existing Files
        // --------------------------

        foreach (var file in Directory.GetFiles(Path.Combine(temp, target), "*", SearchOption.AllDirectories))
        {
            var relative = file.Replace(Path.Combine(temp, target), string.Empty).TrimStart(Path.DirectorySeparatorChar);
            var destination = Path.Combine(parent, target, relative);
            Copy(file, destination);
        }

        // Delete the Temp Folder
        // ----------------------

        Directory.Delete(temp, true);

        // Restore the SQLite3 Cache File
        // ------------------------------

        var backups = Directory.GetDirectories(Path.Combine(parent, "Backups")).OrderByDescending(x => x).ToList();
        for (var i = 2; i < backups.Count; i++)
        {
            Directory.Delete(backups[i], true);
        }

        var backupSQLite = Directory.GetFiles(backups[0], sqlite, SearchOption.AllDirectories).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(backupSQLite))
        {
            Console.WriteLine("The backup of the SQLite3 cache file is not found");
        }
        else
        {
            Copy(backupSQLite, Path.Combine(parent, target, sqlite));
        }

        // Start the Client App
        // --------------------

        var process = new Process
        {
            StartInfo = new()
            {
                FileName = Path.Combine(parent, target, exeSLA),
                Arguments = "--no-start",
                UseShellExecute = true
            }
        };
        process.Start();

        // Delete the Update Package
        // -------------------------

        File.Delete(path);

        // Exit
        // ----

        Console.WriteLine("The update is successfully applied");
    }

    private static void Copy(string source, string destination)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(source, destination, true);
    }
}