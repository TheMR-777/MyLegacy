using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace MySpace;

internal partial class Program
{
    private const bool is_windows = true;

    private static void Main()
    {
        while (true)
        {
            string title, pid, name;
            if (is_windows)
            {
                var handle = GetForegroundWindow();
                var text = new StringBuilder(MAX_TITLE_LENGTH);
                if (GetWindowText(handle, text, MAX_TITLE_LENGTH) > 0)
                {
                    GetWindowThreadProcessId(handle, out var pid_int);
                    var proc = Process.GetProcessById((int)pid_int);

                    title = text.ToString();
                    pid = pid_int.ToString();
                    name = proc.ProcessName;
                }
                else
                {
                    title = "";
                    pid = "";
                    name = "";
                }
            }
            else
            {
                var (pid_int, name_str) = GetActiveProcessMac();
                var title_str = GetActiveWindowTitleMac();

                title = title_str;
                pid = pid_int.ToString();
                name = name_str;
            }

            Console.WriteLine("Title : " + title);
            Console.WriteLine("PID   : " + pid);
            Console.WriteLine("Name  : " + name);
            Console.WriteLine();
            Thread.Sleep(5 * 1000);
        }
    }

    // Windows API
    // -----------

    [LibraryImport("user32.dll")]
    public static partial IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    private const int MAX_TITLE_LENGTH = 256;

    // macOS API
    // ---------

    private static (int, string) GetActiveProcessMac()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = "-c \"osascript -e 'tell application \\\"System Events\\\" to unix id of (every process whose frontmost is true)'\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var p = Process.Start(psi);
        p?.WaitForExit();

        if (p?.ExitCode != 0)
            throw new Exception("osascript execution failed");

        var pid = int.Parse(p.StandardOutput.ReadToEnd().Trim());

        return (pid, Process.GetProcessById(pid).ProcessName);
    }

    private static string GetActiveWindowTitleMac()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = "-c \"osascript -e 'tell application \\\"System Events\\\" to get the title of every window of (every process whose frontmost is true)'\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var p = Process.Start(psi);
        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception("osascript execution failed");

        return p.StandardOutput.ReadToEnd().Trim();
    }
}