using System.Runtime.InteropServices;

namespace Playground;

public static partial class MacOSPowerManagement
{
    private static readonly IntPtr kIOPMAssertionLevelOn = (IntPtr)1;
    private static uint _sleepAssertionId = 0;

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static extern IOReturn IOPMAssertionCreateWithName(
        CFStringRef assertionType,
        IntPtr assertionLevel,
        CFStringRef assertionName,
        out uint assertionId);

    [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static partial IOReturn IOPMAssertionRelease(uint assertionId);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", StringMarshalling = StringMarshalling.Utf8)]
    private static partial CFStringRef CFStringCreateWithCString(IntPtr alloc, string str, ulong encoding);

    private static CFStringRef CFStringCreate(string str) => CFStringCreateWithCString(IntPtr.Zero, str, 0x08000100 /* kCFStringEncodingUTF8 */);

    public static void Hibernation(bool allow)
    {
        if (!allow && _sleepAssertionId == 0)
        {
            IOPMAssertionCreateWithName(
                CFStringCreate("PreventUserIdleSystemSleep"),
                kIOPMAssertionLevelOn,
                CFStringCreate("Fintech Monitoring App Needs to Run"),
                out _sleepAssertionId);
            Console.WriteLine("Sleep prevention is now active.");
        }
        else if (_sleepAssertionId != 0)
        {
            IOPMAssertionRelease(_sleepAssertionId);
            _sleepAssertionId = 0;
            Console.WriteLine("Sleep prevention is now inactive.");
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct CFStringRef
{
    private IntPtr handle;
    public static implicit operator IntPtr(CFStringRef value) => value.handle;
    public static implicit operator CFStringRef(IntPtr value) => new() { handle = value };
}

public enum IOReturn : int
{
    Success = 0,
    Error = -1
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Preventing sleep for 1 hour...");
        MacOSPowerManagement.Hibernation(false);

        DateTime previousTimestamp = DateTime.Now;
        const int totalDurationInSeconds = 3600;  // Total duration for the test in seconds (1 hour)
        const int intervalInSeconds = 10;         // Interval between checks in seconds

        int numberOfIterations = totalDurationInSeconds / intervalInSeconds;

        for (int i = 0; i < numberOfIterations; i++)
        {
            DateTime currentTimestamp = DateTime.Now;
            TimeSpan timeDifference = currentTimestamp - previousTimestamp;

            // Check if the time difference is significantly more than the interval
            if (timeDifference.TotalSeconds > intervalInSeconds * 1.5)  // Allowing a little grace period
            {
                Console.WriteLine($"Anomaly Detected: Time difference is {timeDifference.TotalSeconds} seconds at {currentTimestamp:HH:mm:ss}");
            }
            else
            {
                Console.WriteLine($"Timestamp: {currentTimestamp:HH:mm:ss}, Interval: {timeDifference.TotalSeconds} seconds");
            }

            previousTimestamp = currentTimestamp;
            Thread.Sleep(intervalInSeconds * 1000);
        }

        MacOSPowerManagement.Hibernation(true);
        Console.WriteLine("Sleep is now allowed. Exiting...");
    }
}
