using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyPlayground_C_
{
    internal static partial class NetworkDriveAccessor
    {
        private const string TheDrive = "172.16.70.75";
        private const string MyVolume = "amsNet";
        private const string MyDomain = "ACE";
        private const string Username = "asc";
        private const string Password = "asc1234";
        private const string FilePath =
#if WIN
            @"SLA\test.txt";
#elif MAC
            "SLA/test.txt";
#endif

#if WIN
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
#if WIN
            var netResource = new NETRESOURCEA
            {
                dwType = 1, // RESOURCETYPE_ANY
                lpLocalName = IntPtr.Zero,
                lpRemoteName = Marshal.StringToHGlobalAnsi(GetNetworkDrivePath),
                lpProvider = IntPtr.Zero
            };

            try
            {
                var netResourcePtr = Marshal.AllocHGlobal(Marshal.SizeOf(netResource));
                Marshal.StructureToPtr(netResource, netResourcePtr, false);

                var result = WNetAddConnection2A(netResourcePtr, Password, $"{Username}@{MyDomain}.com", 0);
                if (result is 0 or 1219) return;
                throw new Exception($"ERROR: Failed to connect to network drive. Error code: {result}");
            }
            finally
            {
                Marshal.FreeHGlobal(netResource.lpRemoteName);
            }
#elif MAC
            var mountPoint = GetNetworkDrivePath;
            if (IsMounted(mountPoint)) return;
            if (!Directory.Exists(mountPoint)) Directory.CreateDirectory(mountPoint);

            // var mountCommand = $"echo '{Password}' | mount_smbfs //{Username}@{TheDrive}/{MyVolume} \\\"{mountPoint}\\\"";
            var mountCommand = $"mount_smbfs //{Username}:{Password}@{TheDrive}/{MyVolume} \\\"{mountPoint}\\\"";
            ExecuteCommand(mountCommand);
#endif
        }

        private static void CancelConnection()
        {
#if WIN
            WNetCancelConnection2A(GetNetworkDrivePath, 0, true);
#elif MAC
            var mountPoint = GetNetworkDrivePath;
            if (!IsMounted(mountPoint)) return;

            var unmountCommand = $"umount {mountPoint}";
            ExecuteCommand(unmountCommand);
            Directory.Delete(mountPoint);
#endif
        }

        private static void TryAccessingFile()
        {
            try
            {
                var fileContent = File.ReadAllText(Path.Combine(GetNetworkDrivePath, FilePath));
                Console.WriteLine(fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        // Utilities
        // ---------

        private static string GetNetworkDrivePath
#if WIN
            => $@"\\{TheDrive}\{MyVolume}";
#elif MAC
            => $"mnt/{MyVolume}-777";
#endif

        private static void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("sh", $"-c \"{command}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo) ?? throw new Exception("Failed to start process.");
            process.WaitForExit();
            if (process.ExitCode == 0) return;
            var error = process.StandardError.ReadToEnd();
            throw new Exception($"Command execution failed with exit code {process.ExitCode}: {error}");
        }

#if MAC
        private static bool IsMounted(string mountPoint)
        {
            var checkMountCommand = $"mount | grep \\\"{mountPoint}\\\"";
            var processInfo = new ProcessStartInfo("sh", $"-c \"{checkMountCommand}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processInfo) ?? throw new Exception("Failed to start process.");
            process.WaitForExit();
            return !string.IsNullOrEmpty(process.StandardOutput.ReadToEnd());
        }
#endif
    }
}
