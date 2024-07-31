using System.Net;
using System.Runtime.InteropServices;

namespace MyPlayground_C_;

internal static partial class NetworkDriveAccessor
{
    private const string NetworkDrivePath = @"\\172.16.70.75\amsNet";
    private const string Username = "asc@ace.com";
    private const string Password = "asc1234";
    private const string Domain = "ACE";
    private const string FilePath = @"SLA\test.txt";

    [LibraryImport("mpr.dll", EntryPoint = "WNetAddConnection2A")]
    private static partial int WNetAddConnection2A(IntPtr netResource, [MarshalAs(UnmanagedType.LPStr)] string password, [MarshalAs(UnmanagedType.LPStr)] string username, int flags);

    [LibraryImport("mpr.dll", EntryPoint = "WNetCancelConnection2A")]
    private static partial int WNetCancelConnection2A([MarshalAs(UnmanagedType.LPStr)] string name, int flags, [MarshalAs(UnmanagedType.Bool)] bool force);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct NETRESOURCEA
    {
        public int dwScope;
        public int dwType;
        public int dwDisplayType;
        public int dwUsage;
        public IntPtr lpLocalName;
        public IntPtr lpRemoteName;
        public IntPtr lpComment;
        public IntPtr lpProvider;
    }

    public static void Main()
    {
        Console.WriteLine("Creating connection to network drive...");
        CreateConnection();

        Console.WriteLine();
        Console.WriteLine("Testing Method 1: Using WNetAddConnection2A");
        TestMethod1();

        Console.WriteLine();
        Console.WriteLine("Testing Method 2: Using WebClient");
        TestMethod2();

        Console.WriteLine();
        Console.WriteLine("Testing Method 3: Using FileStream with NetworkCredential");
        TestMethod3();

        Console.WriteLine();
        Console.WriteLine("Connection to network drive is being cancelled...");
        CancelConnection();

        Console.ReadLine();
    }

    private static void CreateConnection()
    {
        var netResource = new NETRESOURCEA
        {
            dwType = 1, // RESOURCETYPE_ANY
            lpLocalName = IntPtr.Zero,
            lpRemoteName = Marshal.StringToHGlobalAnsi(NetworkDrivePath),
            lpProvider = IntPtr.Zero
        };

        try
        {
            var netResourcePtr = Marshal.AllocHGlobal(Marshal.SizeOf(netResource));
            Marshal.StructureToPtr(netResource, netResourcePtr, false);

            var result = WNetAddConnection2A(netResourcePtr, Password, Username, 0);
            if (result is 0 or 1219) return;
            throw new Exception($"ERROR: Failed to connect to network drive. Error code: {result}");
        }
        finally
        {
            Marshal.FreeHGlobal(netResource.lpRemoteName);
        }
    }

    private static void CancelConnection()
    {
        WNetCancelConnection2A(NetworkDrivePath, 0, true);
    }

    private static void TestMethod1()
    {
        try
        {
            var fileContent = File.ReadAllText(Path.Combine(NetworkDrivePath, FilePath));
            Console.WriteLine(fileContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    private static void TestMethod2()
    {
        try
        {
            var credential = new NetworkCredential(Username, Password);
            var networkDriveUri = new Uri($"file://{NetworkDrivePath.Replace("\\", "/")}");
            var fileUri = new Uri($"{networkDriveUri.AbsoluteUri}/{FilePath.Replace("\\", "/")}");

            using var client = new WebClient { Credentials = credential };
            var fileContents = client.DownloadString(fileUri);
            Console.WriteLine(fileContents);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    private static void TestMethod3()
    {
        try
        {
            using var fileStream = new FileStream(Path.Combine(NetworkDrivePath, FilePath), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            using var streamReader = new StreamReader(fileStream);
            var fileContent = streamReader.ReadToEnd();
            Console.WriteLine(fileContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }
}
