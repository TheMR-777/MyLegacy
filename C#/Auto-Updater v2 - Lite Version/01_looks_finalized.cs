using System.Diagnostics;

namespace MyEvolver;

static class Evolver
{
    private static readonly string parent = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;
    private static string backupDirectory = string.Empty;
    private const string Backup = "Backups";
    private const string MyName = "SLA-Remake";
    private const string Target = $"{MyName} - Windows";
    private const string Sqlite = $"{MyName}.sqlite";
    private const string ExeSLA = $"{MyName}.exe";

    public static void Main(string[] arguments)
    {
        try
        {
            Log("Evolver started.");

            var path = ValidateArguments(arguments);
            TerminateTheClientApp();
            backupDirectory = BackupCurrentVersion();
            MoveUpdateContent(path);
            RestoreClientDatabase();
            DisposeUnusedBackups(3);
            InitiateTheClientApp();
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
            LogError("The path of the update package is not provided. Usage: evolver <path-to-update-package>");
            throw new ArgumentException("Invalid arguments.");
        }

        var path = arguments.FirstOrDefault() ?? string.Empty;
        if (!Directory.Exists(path))
        {
            LogError($"The provided path is not valid: {path}");
            throw new DirectoryNotFoundException("Invalid path.");
        }

        return path;
    }

    private static void TerminateTheClientApp()
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
            var backup = Path.Combine(parent, Backup, $"{Target} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}");
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

    private static void MoveUpdateContent(string path)
    {
        try
        {
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                var relative = file.Replace(path, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                var destination = Path.Combine(parent, Target, relative);
                Copy(file, destination);
            }
            //Directory.Delete(path, true);
            Log("Update content moved to installation folder.");
        }
        catch (Exception ex)
        {
            LogError($"Failed to move update content: {ex.Message}");
            throw new IOException("Move update content process failed.", ex);
        }
    }

    private static void RestoreClientDatabase()
    {
        try
        {
            var backupSQLite = Directory.GetFiles(backupDirectory, Sqlite, SearchOption.AllDirectories).Max();
            if (string.IsNullOrEmpty(backupSQLite))
            {
                LogError("The backup of the SQLite3 cache file is not found.\n IN: " + backupDirectory);
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

    private static void DisposeUnusedBackups(int keep)
    {
        try
        {
            for (var backups = Directory.GetDirectories(Path.Combine(parent, Backup)).OrderDescending().ToList(); keep < backups.Count; keep++)
            {
                Directory.Delete(backups[keep], true);
                Log($"Old backup deleted: {backups[keep]}");
            }
        }
        catch (Exception ex)
        {
            LogError($"Failed to clean up old backups: {ex.Message}");
            throw;
        }
    }

    private static void InitiateTheClientApp()
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
            Directory.Delete(path, true);
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
            if (string.IsNullOrEmpty(backupDirectory) || !Directory.Exists(backupDirectory))
            {
                LogError("No valid backup location found. Cannot restore previous version.");
                return;
            }

            foreach (var file in Directory.GetFiles(backupDirectory, "*", SearchOption.AllDirectories))
            {
                var relative = file.Replace(backupDirectory, string.Empty).TrimStart(Path.DirectorySeparatorChar);
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

    private static void Copy(string source, string destination)
    {
        var message = $"\n\n SOURCE: {source}\n DESTIN: {destination}\n";
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