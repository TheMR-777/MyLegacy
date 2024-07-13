using System.Diagnostics;

public class ProcessKiller
{
    public static void KillProcessesContainingName(string pattern)
    {
        var processes = Process.GetProcesses();
        foreach (var process in processes)
        {
            var targeted = 
                process.ProcessName.Contains(pattern, StringComparison.OrdinalIgnoreCase) ||
                process.MainWindowTitle.Contains(pattern, StringComparison.OrdinalIgnoreCase);
            if (targeted)
            {
                process.Kill();
                Console.WriteLine($"Killed process {process.ProcessName} (ID: {process.Id})");
            }
        }
    }

    public static void Main()
    {
        KillProcessesContainingName("SLA-Remake");
    }
}
