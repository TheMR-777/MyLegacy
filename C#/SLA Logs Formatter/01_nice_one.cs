namespace MySpace
{
    class MyClass
    {
        private readonly static string _path = @"C:\Users\ASC\Downloads\SLA v3 with TheMR\Uninstall.log";

        public static void Main()
        {
            // Split the Log with "|\n" as the delimiter, and don't include the empty entries.
            var logs = File.ReadAllText(_path).Split(new string[] { "|\r\n" }, StringSplitOptions.RemoveEmptyEntries).TakeLast(3);

            // Loop through the logs and print them to the console.
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
