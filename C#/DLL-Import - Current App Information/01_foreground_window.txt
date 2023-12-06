using System.Text;
using System.Runtime.InteropServices;

namespace MySpace;

internal partial class Program
{
    [LibraryImport("user32.dll")]
    private static partial IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    private const int MAX_TITLE_LENGTH = 256;

    private static void Main()
    {
        while (true)
        {
            var handle = GetForegroundWindow();
            var title = new StringBuilder(MAX_TITLE_LENGTH);
            GetWindowText(handle, title, MAX_TITLE_LENGTH);
            Console.WriteLine(title);
            Thread.Sleep(10 * 1000);
        }
    }
}