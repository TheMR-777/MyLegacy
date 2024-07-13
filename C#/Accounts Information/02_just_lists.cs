using System;
using System.Linq;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            // 1. Computer/System name
            Console.WriteLine("Computer name  : " + Environment.MachineName);

            // 2. Currently Logged-In username
            Console.WriteLine("Current user   : " + WindowsIdentity.GetCurrent().Name);

            // 3. The Run-As name
            Console.WriteLine("Run as user    : " + Environment.UserName);

            // 4. List of Admin accounts, separated by comma
            var admins = GetGroupMembers("Administrators");
            Console.WriteLine("Admin accounts : " + string.Join(", ", admins));

            // 5. List of all other remaining accounts
            var users = GetGroupMembers("Users").Except(admins);
            Console.WriteLine("Other accounts : " + string.Join(", ", users));
        }

        // A helper method to get the members of a local group
        static string[] GetGroupMembers(string groupName)
        {
            using (var context = new PrincipalContext(ContextType.Machine))
            using (var group = GroupPrincipal.FindByIdentity(context, groupName))
            {
                return group.GetMembers()
                    .Select(m => m.SamAccountName)
                    .ToArray();
            }
        }
    }
}