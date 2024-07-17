using System.Diagnostics;
using System.IO.Compression;

namespace MyEvolver;

static class Evolver
{
    private static readonly string parent = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
    private const string MyName = "SLA-Remake";
    private const string Target = $"{MyName} - Windows";
    private const string Sqlite = $"{MyName}.sqlite";
    private const string ExeSLA = $"{MyName}.exe";
    private static string backupLocation = string.Empty;

    public static void Main(string[] arguments)
    {
        try
        {
            Log("Evolver started.");

            var path = ValidateArguments(arguments);
            TerminateClientApp();
            backupLocation = BackupCurrentVersion();
            var temp = UnzipArchive(path);
            ReplaceExistingFiles(temp);
            RestoreSQLiteCacheFile();
            CleanUpOldBackups();
            StartClientApp();
            DisposeTheUpdate(path);

            Log("Evolver completed.");
        }
        catch (Exception ex)
        {
            LogError($"An unexpected error occurred: {ex.Message}");
            Log("Restoring the previous version.");

            RestorePreviousVersion();
        }
    }

    // Steps

    private static string ValidateArguments(string[] arguments)
    {
        if (arguments.Length != 1)
        {
            LogError("The path of zip-file is not provided. Usage: evolver <path-to-zip-archive>");
            throw new ArgumentException("Invalid arguments.");
        }

        var path = arguments.FirstOrDefault() ?? string.Empty;
        if (!File.Exists(path))
        {
            LogError($"The provided path is not valid: {path}");
            throw new FileNotFoundException("Invalid path.");
        }

        if (!IsZipFile(path))
        {
            LogError("The provided path is not a valid zip file.");
            throw new Exception("Provided path is not a valid zip file.");
        }

        return path;
    }

    private static void TerminateClientApp()
    {
        try
        {
            var processes = Process.GetProcesses()
                .Where(p => p.ProcessName.Contains("SLA-Remake", StringComparison.OrdinalIgnoreCase))
                .ToList();
            processes.ForEach(p => p.Kill());
            Log("Client app terminated.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to terminate client app: {ex.Message}");
            throw new ApplicationException("Could not terminate client app.", ex);
        }
    }

    private static string BackupCurrentVersion()
    {
        try
        {
            var backup = Path.Combine(parent, "Backups", $"{Target} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}");
            Directory.CreateDirectory(backup);
            foreach (var file in Directory.GetFiles(Path.Combine(parent, Target), "*", SearchOption.AllDirectories))
            {
                var relative = file.Replace(Path.Combine(parent, Target), string.Empty).TrimStart(Path.DirectorySeparatorChar);
                var destination = Path.Combine(backup, relative);
                Copy(file, destination);
            }
            Log("Current version backed up.");

            return backup;
        }
        catch (Exception ex)
        {
            LogError($"Failed to backup current version: {ex.Message}");
            throw new IOException("Backup process failed.", ex);
        }
    }

    private static string UnzipArchive(string path)
    {
        try
        {
            var temp = Path.Combine(parent, "Temp");
            ZipFile.ExtractToDirectory(path, temp, true);
            Log("Update package unzipped.");

            if (!File.Exists(Path.Combine(temp, Target, ExeSLA)))
            {
                throw new InvalidDataException("The provided zip-file is not valid. Required files are missing.");
            }

            return temp;
        }
        catch (Exception ex)
        {
            LogError($"Failed to unzip archive: {ex.Message}");
            throw new IOException("Unzip process failed.", ex);
        }
    }

    private static void ReplaceExistingFiles(string temp)
    {
        try
        {
            foreach (var file in Directory.GetFiles(Path.Combine(temp, Target), "*", SearchOption.AllDirectories))
            {
                var relative = file.Replace(Path.Combine(temp, Target), string.Empty).TrimStart(Path.DirectorySeparatorChar);
                var destination = Path.Combine(parent, Target, relative);
                Copy(file, destination);
            }
            Directory.Delete(temp, true);
            Log("Existing files replaced with new files.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to replace existing files: {ex.Message}");
            throw new IOException("File replacement process failed.", ex);
        }
    }

    private static void RestoreSQLiteCacheFile()
    {
        try
        {
            var backupSQLite = Directory.GetFiles(backupLocation, Sqlite, SearchOption.AllDirectories).Max();
            if (string.IsNullOrEmpty(backupSQLite))
            {
                LogError("The backup of the SQLite3 cache file is not found.\n IN: " + backupLocation);
            }
            else
            {
                Copy(backupSQLite, Path.Combine(parent, Target, Sqlite));
                Log("SQLite3 cache file restored from backup.\n FROM: " + backupSQLite);
            }
        }
        catch (Exception ex)
        {
            LogError($"Failed to restore SQLite3 cache file: {ex.Message}");
            throw new IOException("SQLite cache restore process failed.", ex);
        }
    }

    private static void CleanUpOldBackups()
    {
        try
        {
            var backups = Directory.GetDirectories(Path.Combine(parent, "Backups")).OrderByDescending(x => x).ToList();
            for (var i = 2; i < backups.Count; i++)
            {
                Directory.Delete(backups[i], true);
                Log($"Old backup deleted: {backups[i]}");
            }
        }
        catch (Exception ex)
        {
            LogError($"Failed to clean up old backups: {ex.Message}");
            throw;
        }
    }

    private static void StartClientApp()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(parent, Target, ExeSLA),
                    Arguments = "--no-start",
                    UseShellExecute = true
                }
            };
            process.Start();
            Log("Client app started with '--no-start' argument.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to start client app: {ex.Message}");
            throw new ApplicationException("Could not start client app.", ex);
        }
    }

    private static void DisposeTheUpdate(string path)
    {
        try
        {
            File.Delete(path);
            Log("The update package is deleted.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to delete the update package: {ex.Message}");
            throw new IOException("Could not delete update package.", ex);
        }
    }

    private static void RestorePreviousVersion()
    {
        try
        {
            if (string.IsNullOrEmpty(backupLocation) || !Directory.Exists(backupLocation))
            {
                LogError("No valid backup location found. Cannot restore previous version.");
                return;
            }

            foreach (var file in Directory.GetFiles(backupLocation, "*", SearchOption.AllDirectories))
            {
                var relative = file.Replace(backupLocation, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                var destination = Path.Combine(parent, Target, relative);
                Copy(file, destination);
            }

            Log("Previous version restored successfully.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to restore previous version: {ex.Message}");
            throw new IOException("Restore previous version process failed.", ex);
        }
    }

    // Utilities

    private static bool IsZipFile(string path)
    {
        try
        {
            using var file = File.OpenRead(path);
            using var archive = new ZipArchive(file, ZipArchiveMode.Read, true);
            return archive.Entries.Any();
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    private static void Copy(string source, string destination)
    {
        var message = $"\n SOURCE: {source}\n DESTIN: {destination}";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(source, destination, true);
            Log("File copied successfully" + message);
        }
        catch (Exception ex)
        {
            message += $"\n REASON {ex.Message}";
            throw new IOException("Failed to copy file" + message, ex);
        }
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}");
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}");
    }
}
