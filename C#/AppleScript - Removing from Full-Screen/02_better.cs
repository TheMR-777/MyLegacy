using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Improved AppleScript to directly toggle full-screen mode of the frontmost application
        string script = @"
tell application ""System Events""
    set frontmostProcess to first process where it is frontmost
    set frontmostWindow to window 1 of frontmostProcess
    set fullscreenEnabled to value of attribute ""AXFullScreen"" of frontmostWindow
    if fullscreenEnabled is true then
        set value of attribute ""AXFullScreen"" of frontmostWindow to false
    end if
end tell";

        while (true)
        {
            Console.WriteLine("Waiting 10 seconds to toggle full-screen mode...");
            await Task.Delay(10000);  // Wait for 10 seconds
            // Run the AppleScript from a temporary file
            RunAppleScriptFromFile(script);
        }
    }

    static void RunAppleScriptFromFile(string script)
    {
        string path = Path.Combine(Path.GetTempPath(), "tempScript.scpt");
        File.WriteAllText(path, script);

        // Create and start the process to run the AppleScript
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "osascript",
                Arguments = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Error: {error}");
        }
        else
        {
            Console.WriteLine($"Result: {result}");
        }
        File.Delete(path);
    }
}
