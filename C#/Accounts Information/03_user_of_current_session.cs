using System;
using System.Runtime.InteropServices;

partial class Program
{
    static void Main(string[] args)
    {
        uint sessionId = WTSGetActiveConsoleSessionId();

        if (sessionId == 0xFFFFFFFF)
        {
            Console.WriteLine("No Active Session");
        }
        else
        {
            var buffer = IntPtr.Zero;
            var username = "Not Available";

            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out _))
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
            }

            Console.WriteLine($"Active Session ID: {sessionId}, Username: {username}");
            Console.ReadLine();
        }
    }

    enum WTS_INFO_CLASS
    {
        WTSUserName = 5,
        // include others as needed
    }

    [DllImport("Wtsapi32.dll")]
    static extern bool WTSQuerySessionInformation(IntPtr hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

    [LibraryImport("Wtsapi32.dll")]
    public static partial void WTSFreeMemory(IntPtr pointer);

    [LibraryImport("Kernel32.dll")]
    private static partial uint WTSGetActiveConsoleSessionId();
}