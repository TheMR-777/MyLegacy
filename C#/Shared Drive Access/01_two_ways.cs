using System.Net;
using System.Runtime.InteropServices;

namespace MyPlayground_C_;

internal static class NetworkDriveAccessor
{
    private const string NetworkDrivePath = @"\\172.16.70.75\amsNet";
    private const string Username = "***";
    private const string Password = "***";
    private const string Domain = "***";
    private const string FilePath = @"SLA\test.txt";

    [DllImport("mpr.dll")]
    private static extern int WNetAddConnection2A(ref NETRESOURCEA netResource, string password, string username, int flags);

    [DllImport("mpr.dll")]
    private static extern int WNetCancelConnection2A(string name, int flags, bool force);

    [StructLayout(LayoutKind.Sequential)]
    private struct NETRESOURCEA
    {
        public int dwScope;
        public int dwType;
        public int dwDisplayType;
        public int dwUsage;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpLocalName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpRemoteName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpComment;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpProvider;
    }

    public static void Main()
    {
        Console.WriteLine("Testing Method 1: Using WNetAddConnection2A");

        try
        {
            // Create a new NETRESOURCEA struct
            var netResource = new NETRESOURCEA
            {
                dwType = 1, // RESOURCETYPE_ANY
                lpLocalName = null,
                lpRemoteName = NetworkDrivePath,
                lpProvider = null
            };

            // Add the connection
            var result = WNetAddConnection2A(ref netResource, Password, $"{Domain}\\{Username}", 0);
            if (result != 0 && result != 1219)
            {
                Console.WriteLine($"Failed to connect to network drive. Error code: {result}");
            }
            else try
            {
                // Read the file
                var fileContent = File.ReadAllText(Path.Combine(NetworkDrivePath, FilePath));
                Console.WriteLine(fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file. Error: {ex.Message}");
            }
            finally
            {
                // Cancel the connection
                WNetCancelConnection2A(NetworkDrivePath, 0, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("\nTesting Method 2: Using WebClient");

        try
        {
            // Create a new credential object
            var credential = new NetworkCredential(Username, Password);

            // Create a new Uri object for the network drive
            var networkDriveUri = new Uri(NetworkDrivePath);

            // Create a new Uri object for the file
            var fileUri = new Uri(Path.Combine(networkDriveUri.AbsoluteUri, FilePath));

            // Use the credential to access the network drive
            using var client = new WebClient { Credentials = credential };

            // Read the file contents
            var fileContents = client.DownloadString(fileUri);

            // Print the file contents
            Console.WriteLine(fileContents);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
