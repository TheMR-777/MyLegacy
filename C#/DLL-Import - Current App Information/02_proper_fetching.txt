using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace MySpace;

internal partial class Program
{
    private static void Main()
    {
        while (true)
        {
            var handle = GetForegroundWindow();
            var text = new StringBuilder(MAX_TITLE_LENGTH);
            if (GetWindowText(handle, text, MAX_TITLE_LENGTH) > 0)
            {
                GetWindowThreadProcessId(handle, out var pid);
                var proc = Process.GetProcessById((int)pid);

                Console.WriteLine("Title : " + text);
                Console.WriteLine("PID   : " + pid);
                Console.WriteLine("Name  : " + proc.ProcessName);
                Console.WriteLine("Start@: " + proc.StartTime);
                Console.WriteLine();
            }
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
}