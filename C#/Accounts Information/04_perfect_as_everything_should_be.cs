using System;
using System.Linq;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;

namespace ConsoleApp
{
    partial class Program
    {
        public static void Main()
        {
            // 1. Computer/System name
            Console.WriteLine("Computer name  : " + Environment.MachineName);

            // 2. Currently Logged-In username
            Console.WriteLine("Logged-in user : " + GetUserFromSession());

            // 3. The Run-As name
            Console.WriteLine("Run-as user    : " + Environment.UserName);

            // 4. List of Admin accounts, separated by comma
            var admins = GetGroupMembers("Administrators");
            Console.WriteLine("Admin accounts : " + string.Join(", ", admins));

            // 5. List of all other remaining accounts
            var users = GetGroupMembers("Users").Except(admins);
            Console.WriteLine("Other accounts : " + string.Join(", ", users));

            // ...
            Console.ReadKey();
        }

        // A helper method to get the members of a local group
        static string[] GetGroupMembers(string groupName)
        {
            using var context = new PrincipalContext(ContextType.Machine);
            using var group = GroupPrincipal.FindByIdentity(context, groupName);
            return group.GetMembers()
                .Select(m => m.SamAccountName)
                .ToArray();
        }

        // A helper method to get the currently logged-in user
        static string GetUserFromSession()
        {
            var sessionId = WTSGetActiveConsoleSessionId();
            var username = "N/A";

            if (sessionId == 0xFFFFFFFF)
                return username;

            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out nint buffer, out _))
            {
                username = Marshal.PtrToStringAnsi(buffer) ?? username;
                WTSFreeMemory(buffer);
            }
            return username;
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
}
