using System.Diagnostics;

namespace MyPlayground_C_;

internal static class NetworkDriveAccessor
{
    private const string TheDrive = "172.16.70.75";
    private const string MyVolume = "amsNet";
    private const string Username = "asc";
    private const string Password = "asc1234";
    private const string FilePath =
#if WIN
        @"SLA\test.txt";
#elif MAC
        "SLA/test.txt";
#endif

    public static void Main()
    {
        Console.WriteLine("Creating connection to network drive...");
        CreateConnection();

        Console.WriteLine();
        Console.WriteLine("Accessing the File...");
        TryAccessingFile();

        Console.WriteLine();
        Console.WriteLine("Connection to network drive is being cancelled...");
        CancelConnection();

        Console.ReadLine();
    }

    private static void CreateConnection()
    {
        var smb = GetNetworkDrivePath();
        if (IsMounted(smb)) return;

        var command =
#if WIN
            $"net use {smb} /user:{Username} {Password}";
#elif MAC
            $"mount_smbfs //{Username}:{Password}@{TheDrive}/{MyVolume} \"{smb}\"";
        if (!Directory.Exists(smb)) Directory.CreateDirectory(smb);
#endif

        ExecuteCommand(command);
    }

    private static void CancelConnection()
    {
        var networkDrivePath = GetNetworkDrivePath();
        if (!IsMounted(networkDrivePath)) return;

        var command =
#if WIN
            $"net use {networkDrivePath} /delete /y";
#elif MAC
            $"diskutil unmount force \\\"{networkDrivePath}\\\"";
#endif
        ExecuteCommand(command);
    }

    private static void TryAccessingFile()
    {
        try
        {
            var filePath = Path.Combine(GetNetworkDrivePath(), FilePath);
            var fileContent = File.ReadAllText(filePath);
            Console.WriteLine(fileContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    private static string GetNetworkDrivePath()
    {
#if WIN
        return $@"\\{TheDrive}\{MyVolume}";
#elif MAC
        return $"mnt/{MyVolume}-777";
#endif
    }

    private static void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo
#if WIN
        ("cmd.exe", $"/c {command}")
#elif MAC
        ("sh", $"-c \"{command}\"")
#endif
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processInfo) ?? throw new Exception("Failed to start process.");
        process.WaitForExit();
        if (process.ExitCode is 0 or 1219) return;

        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode == 2 && error.Contains("1219")) return;
        throw new Exception($"Command execution failed with exit code {process.ExitCode}: {error}");
    }

    private static bool IsMounted(string mountPoint)
    {
        var processInfo = new ProcessStartInfo
#if WIN
        ("cmd.exe", $"/c net use | findstr {mountPoint}")
#elif MAC
        ("sh", $"-c \"mount | grep \\\"{mountPoint}\\\"\"")
#endif
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processInfo) ?? throw new Exception("Failed to start process.");
        process.WaitForExit();
        return !string.IsNullOrEmpty(process.StandardOutput.ReadToEnd());
    }
}