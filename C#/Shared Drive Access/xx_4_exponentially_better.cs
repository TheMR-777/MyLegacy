using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MyPlayground_C_;

internal static class NetworkDriveAccessor
{
    private const string Username = "asc";
    private const string Password = "asc1234";
    private const string Domain = "ACE";

    public static void Main()
    {
        const string smbUrl = "smb://172.16.70.75/amsNet/SLA/test.txt";

        Console.WriteLine("Creating connection to network drive...");
        CreateConnection(smbUrl);

        Console.WriteLine("\nAccessing the File...");
        TryAccessingFile(smbUrl);

        Console.WriteLine("\nConnection to network drive is being cancelled...");
        CancelConnection(smbUrl);

        Console.ReadLine();
    }

    private static void TryAccessingFile(string smbUrl)
    {
        try
        {
            var filePath = ConvertSmbUrlToLocalPath(smbUrl);
            var fileContent = File.ReadAllText(filePath);
            Console.WriteLine(fileContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    private static void CreateConnection(string smbUrl)
    {
        var (ip, volume, _) = ParseSmbUrl(smbUrl);
        var mountPoint = GetMountPoint(ip, volume);

        if (IsMounted(mountPoint)) return;

        var command =
#if WIN
            $"net use {mountPoint} /user:{Domain}\\{Username} {Password}";
#elif MAC
            $"mount_smbfs //{Username}:{Password}@{ip}/{volume} \"{mountPoint}\"";
#endif

#if MAC
        if (!Directory.Exists(mountPoint)) Directory.CreateDirectory(mountPoint);
#endif

        var process = ExecuteCommand(command);
        if (process.StandardError.ReadToEnd().Contains("1219")) return;
        VerifyProcess(process);
    }

    private static void CancelConnection(string smbUrl)
    {
        var (ip, volume, _) = ParseSmbUrl(smbUrl);
        var mountPoint = GetMountPoint(ip, volume);

        if (!IsMounted(mountPoint)) return;

        var command =
#if WIN
            $"net use {mountPoint} /delete /y";
#elif MAC
            $"diskutil unmount force \"{mountPoint}\"";
#endif
        VerifyProcess(ExecuteCommand(command));
    }

    private static string ConvertSmbUrlToLocalPath(string smbUrl)
    {
        var (ip, volume, path) = ParseSmbUrl(smbUrl);
        var mountPoint = GetMountPoint(ip, volume);
        return Path.Combine(mountPoint, path);
    }

    private static string GetMountPoint(string ip, string volume) =>
#if WIN
        $@"\\{ip}\{volume}";
#elif MAC
        $"./mnt/{volume}-777";
#endif

    private static bool IsMounted(string mountPoint)
    {
        var command =
#if WIN
            $"net use | findstr {mountPoint}";
#elif MAC
            $"mount | grep \"{mountPoint}\"";
#endif
        var process = ExecuteCommand(command);
        return !string.IsNullOrEmpty(process.StandardOutput.ReadToEnd());
    }

    private static Process ExecuteCommand(string command)
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
        return process;
    }

    private static void VerifyProcess(Process process)
    {
        if (process.ExitCode is 0) return;
        var error = process.StandardError.ReadToEnd();
        throw new Exception($"ERROR: Command execution failed with exit code {process.ExitCode}: {error}");
    }

    private static (string ip, string volume, string path) ParseSmbUrl(string smbUrl)
    {
        var match = Regex.Match(smbUrl, @"smb://([^/]+)/([^/]+)/(.+)");
        if (!match.Success)
        {
            throw new ArgumentException("Invalid SMB URL format", nameof(smbUrl));
        }

        return (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
    }
}
