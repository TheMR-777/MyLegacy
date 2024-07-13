	public static void DeactivateCurrentFullscreenWindow()
	{
#if MAC
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = CommandBase.Bash,
				Arguments = $"-c \"osascript -e 'tell application \\\"System Events\\\" to tell process (name of the first process whose frontmost is true) to if name is not \\\"{Configuration.MyName}\\\" then set value of attribute \\\"AXFullScreen\\\" of its first window to false'\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			}
		};
		process.Start(); process.WaitForExit();
		ReliableUtility.Logger.CreateLog("Altering existing app's Full-Screen");
		ReliableUtility.Logger.CreateLog("Yield : " + process.StandardOutput.ReadToEnd());
		ReliableUtility.Logger.CreateLog("Error : " + process.StandardError.ReadToEnd());
#endif
	}
