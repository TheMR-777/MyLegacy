using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management;
using System.Security.Principal;

namespace GetSystemInfo
{
    class Program
    {
        public static string GetProcessOwner(string processName)
        {
            ManagementObjectCollection processList = new ManagementObjectSearcher($"Select * from Win32_Process Where Name = '{processName}'").Get();
            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                    return argList[0];
            }
            return "NO OWNER";
        }


        static void Main()
        {
            // Get the computer name
            string ctr_name = Environment.MachineName;

            // Get the current username
            string usr_name = WindowsIdentity.GetCurrent().Name;

            // Get the RunAs username
            string run_name = Environment.UserName;

            // Get the admin users
            var admins = new List<string>();
            foreach (var groupMember in (IEnumerable)new DirectoryEntry("WinNT://" + ctr_name).Children.Find("Administrators", "group").Invoke("members", null))
            {
                admins.Add(new DirectoryEntry(groupMember).Name);
            }

            // Check if the current user is an admin user
            bool isAdmin = admins.Contains(usr_name.Split('\\')[1]);

            // Print the results
            // Console.WriteLine(ctr_name + " | " + usr_name + " | " + run_name + " | " + isAdmin + " | " + GetProcessOwner("explorer.exe"));

            Console.WriteLine("Computer Name  : " + ctr_name);
            Console.WriteLine("User Name      : " + usr_name);
            Console.WriteLine("RunAs Name     : " + run_name);
            Console.WriteLine("Is Admin       : " + isAdmin);
            Console.WriteLine("Explorer Owner : " + GetProcessOwner("explorer.exe"));
        }
    }
}
