using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evolver;

public static class Evolver
{
	private static class Configuration
	{
		public static readonly string MyHome = Path.TrimEndingDirectorySeparator(AppDomain.CurrentDomain.BaseDirectory);
		public static readonly string Parent = Directory.GetParent(MyHome)!.FullName;
		public static readonly string TheLog = CreateLogFilename();
		public static EvolverContext context = new();
		public static string BackupDirectory = string.Empty;
		public const string BackupFolder = "Backups";
		public static readonly JsonSerializerOptions Options = new()
		{
			WriteIndented = true,
			TypeInfoResolver = EvolverJsonContext.Default
		};
	}

	public class EvolverContext
	{
		public class ClientContext
		{
			[JsonPropertyName("destination")]
			public string Destination { get; set; } = string.Empty;

			[JsonPropertyName("executable")]
			public string Executable { get; set; } = string.Empty;

			[JsonPropertyName("arguments")]
			public string Arguments { get; set; } = string.Empty;

			[JsonPropertyName("restoration")]
			public List<string> Restoration { get; set; } = new();
		}

		[JsonPropertyName("package_path")]
		public string PackagePath { get; set; } = string.Empty;

		[JsonPropertyName("backups_keep")]
		public int BackupsKeep { get; set; } = 3;

		[JsonPropertyName("client_context")]
		public ClientContext Client { get; set; } = new();

		public void Validate()
		{
			const string message = "Invalid value for: ";

			if (string.IsNullOrEmpty(PackagePath) || !Directory.Exists(PackagePath))
				throw new ArgumentException(message + "package_path: " + PackagePath);

			if (BackupsKeep <= 0)
				throw new ArgumentException(message + "backups_keep. Backup Management is required");

			if (string.IsNullOrEmpty(Client.Destination) || !Directory.Exists(Client.Destination))
				throw new ArgumentException(message + "client_context.destination: " + Client.Destination);

			if (string.IsNullOrEmpty(Client.Executable))
				throw new ArgumentException(message + "client_context.executable.");

			if (Client.Restoration == null || Client.Restoration.Count == 0)
				throw new ArgumentException(message + "client_context.restoration. Client's state restoration is required.");
		}
	}

	public static void Main(string[] arguments)
	{
		Log("Evolver started.");

		// Preparation
		try
		{
			Configuration.context = ParseArguments(arguments);
			TerminateTheClientApp();
		}
		catch (Exception x)
		{
			LogError($"Error occurred in [ Preparation ]: {x.Message}");
			return;
		}

		// Upgradation
		try
		{
			Configuration.BackupDirectory = BackupCurrentVersion();
			MoveUpdateContent();
			RestoreClientContext();
		}
		catch (Exception x)
		{
			LogError($"Error occurred in [ Upgradation ]: {x.Message}");
			Log("Restoring the previous version.");
			RestorePreviousVersion();
		}

		// Disposal
		try
		{
			DisposeUnusedBackups();
			DisposeTheUpdate();
		}
		catch (Exception x)
		{
			LogError($"Error occurred in [ Disposal ]: {x.Message}");
		}

		InitiateTheClientApp();
		Log("Evolver completed.");
	}

	// Steps

	private static EvolverContext ParseArguments(string[] arguments)
	{
		if (arguments.Length != 1)
		{
			LogError("Invalid arguments. Usage: evolver <json-context>");
			LogError("Argument Received: " + string.Join(", ", arguments));
			throw new ArgumentException("Invalid arguments.");
		}

		var jsonBase64Context = arguments[0];
		string jsonContext;
		try
		{
			jsonContext = System.Text.Encoding.UTF8.GetString(
				Convert.FromBase64String(
					jsonBase64Context.Replace('_', '/')
				)
			);
		}
		catch (FormatException x)
		{
			LogError($"Failed Base64 Decoding: {x.Message}");
			LogError("Base64-Encoded Content: " + jsonBase64Context);
			throw new ArgumentException("Invalid Base64 context.", x);
		}

		EvolverContext context;
		try
		{
			context = JsonSerializer.Deserialize(jsonContext, EvolverJsonContext.Default.EvolverContext)!;
			Log($"The JSON Content Received:\n\n{JsonSerializer.Serialize(context, typeof(EvolverContext), Configuration.Options)}\n");
		}
		catch (JsonException x)
		{
			LogError($"Failed JSON Deserialization: {x.Message}");
			LogError("The JSON Content Received: " + jsonContext);
			throw new ArgumentException("Invalid JSON context.", x);
		}

		context.Validate();
		return context;
	}

	private static void TerminateTheClientApp()
	{
		var processes = Process.GetProcesses()
			.Where(p => p.ProcessName.Contains(Path.GetFileNameWithoutExtension(Configuration.context.Client.Executable), StringComparison.OrdinalIgnoreCase))
			.ToList();
		try
		{
			Log("Trying to Terminate: " + Configuration.context.Client.Executable);
			foreach (var process in processes)
			{
				Log("Terminating: " + process.ProcessName);
				process.Kill();
			}
		}
		catch (Exception x)
		{
			LogError($"Failed the Termination: {x.Message}");
			LogError("Processes Found: " + string.Join(", ", processes.Select(p => p.ProcessName)));
			throw new ApplicationException("Could not terminate client app.", x);
		}
	}

	private static string BackupCurrentVersion()
	{
		var source = Configuration.context.Client.Destination;
		var backup = Path.Combine(Configuration.Parent, Configuration.BackupFolder, $"{Path.GetFileName(source)} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}");
		try
		{
			Directory.CreateDirectory(backup);
			foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
			{
				var relative = file.Replace(source, string.Empty).TrimStart(Path.DirectorySeparatorChar);
				var destinationPath = Path.Combine(backup, relative);
				Copy(file, destinationPath);
			}
			Log("Backed-up the Current Version.");
			return backup;
		}
		catch (Exception x)
		{
			LogError($"Failed to Backup the Current Version: {x.Message}{PairedLogMessage(source, backup)}");
			throw new IOException("Backup Failed.", x);
		}
	}

	private static void MoveUpdateContent()
	{
		var src = Path.Combine(Configuration.context.PackagePath, Path.GetFileName(Configuration.context.Client.Destination));
		var dst = Configuration.context.Client.Destination;
		try
		{
			CopyDirectory(src, dst);
			Log("--- Update Successfully Installed.");
		}
		catch (Exception x)
		{
			LogError($"Failed to move Update Content: {x.Message}{PairedLogMessage(src, dst)}");
			throw new IOException("Update Installation Failed.", x);
		}
	}

	private static void RestoreClientContext()
	{
		foreach (var item in Configuration.context.Client.Restoration)
		{
			var backupPath = Path.Combine(Configuration.BackupDirectory, item);
			try
			{
				if (Directory.Exists(backupPath))
				{
					CopyDirectory(backupPath, Path.Combine(Configuration.context.Client.Destination, item));
					Log($"Restored: {item}");
				}
				else if (File.Exists(backupPath))
				{
					Copy(backupPath, Path.Combine(Configuration.context.Client.Destination, item));
					Log($"Restored: {item}");
				}
				else
				{
					LogError($"The Backup not found\n OF: {item}\n IN: {Configuration.BackupDirectory}");
				}
			}
			catch (Exception x)
			{
				LogError($"Failed to Restore: {item} - {x.Message}{PairedLogMessage(backupPath, Configuration.context.Client.Destination)}");
			}
		}
	}

	private static void DisposeUnusedBackups()
	{
		var keep = Configuration.context.BackupsKeep;
		try
		{
			// Manage backup directories
			var backupDirs = Directory.GetDirectories(Path.Combine(Configuration.Parent, Configuration.BackupFolder))
				.OrderDescending()
				.ToList();

			for (var i = keep; i < backupDirs.Count; i++)
			{
				Directory.Delete(backupDirs[i], true);
				Log($"Deleted Backup: {backupDirs[i]}");
			}

			// Manage update logs
			var logFiles = Directory.GetFiles(Configuration.MyHome, "*update.log")
				.OrderDescending()
				.ToList();

			for (var i = keep; i < logFiles.Count; i++)
			{
				File.Delete(logFiles[i]);
				Log($"Deleted Update-Log: {logFiles[i]}");
			}
		}
		catch (Exception x)
		{
			LogError($"Failed to Cleanup: {x.Message}");
			throw;
		}
	}

	private static void InitiateTheClientApp()
	{
		var execute = Path.Combine(Configuration.context.Client.Destination, Configuration.context.Client.Executable);
		var details = new ProcessStartInfo
		{
			UseShellExecute = true,
			CreateNoWindow = true,
		};

		// Granting Permissions
		// --------------------

		details.FileName = "/bin/chmod";
		details.Arguments = $"+x \"{execute}\"";
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) try
		{
			var process = Process.Start(details)!;
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				LogError($"Failed to grant Execution Rights: {process.StandardError.ReadToEnd()}");
				throw new ApplicationException("Could not grant Execution Rights to the Client.");
			}
			Log("Granted Execution Rights");
		}
		catch (Exception x)
		{
			LogError($"Failed to grant Execution Rights: {x.Message}{PairedLogMessageDetail(execute, Configuration.context.Client.Arguments, "APP", "ARG")}");
			throw new ApplicationException("Could not grant Execution Rights to the Client.", x);
		}

		// Initiating the App
		// ------------------

		details.FileName = execute;
		details.Arguments = Configuration.context.Client.Arguments;
		try
		{
			Process.Start(details);
			Log($"Started the Client with: {Configuration.context.Client.Arguments}");
		}
		catch (Exception x)
		{
			LogError($"Failed to Start the Client: {x.Message}{PairedLogMessageDetail(execute, Configuration.context.Client.Arguments, "APP", "ARG")}");
			throw new ApplicationException("Could not start the Client.", x);
		}
	}

	private static void DisposeTheUpdate()
	{
		try
		{
			Directory.Delete(Configuration.context.PackagePath, true);
			Log("Disposed the Update-Package.");
		}
		catch (Exception ex)
		{
			LogError($"Failed to Delete the Update Package: {ex.Message}\n\n PKG: {Configuration.context.PackagePath}");
			throw new IOException("Could not delete update package.", ex);
		}
	}

	private static void RestorePreviousVersion()
	{
		try
		{
			if (string.IsNullOrEmpty(Configuration.BackupDirectory) || !Directory.Exists(Configuration.BackupDirectory))
			{
				LogError("No valid backup location found. Cannot restore previous version.");
				return;
			}

			foreach (var file in Directory.GetFiles(Configuration.BackupDirectory, "*", SearchOption.AllDirectories))
			{
				var relative = file.Replace(Configuration.BackupDirectory, string.Empty).TrimStart(Path.DirectorySeparatorChar);
				var destination = Path.Combine(Configuration.context.Client.Destination, relative);
				Copy(file, destination);
			}

			Log("Restored the Previous Version.");
		}
		catch (Exception ex)
		{
			LogError($"Failed to Restore the Previous: {ex.Message}");
		}
	}

	// Utilities

	private static void CopyDirectory(string source, string destin)
	{
		var message = PairedLogMessage(source, destin);
		try
		{
			foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
			{
				var relative = file.Replace(source, string.Empty).TrimStart(Path.DirectorySeparatorChar);
				var destination = Path.Combine(destin, relative);
				Copy(file, destination);
			}
			Log("Directory copied successfully" + message);
		}
		catch (Exception ex)
		{
			message += $"\n REASON {ex.Message}";
			throw new IOException("Failed to copy directory" + message, ex);
		}
	}

	private static void Copy(string source, string destination)
	{
		var message = PairedLogMessage(source, destination);
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			File.Copy(source, destination, true);
			// Log("File copied successfully" + message);
		}
		catch (Exception ex)
		{
			message += $"\n REASON {ex.Message}";
			throw new IOException("Failed to copy file" + message, ex);
		}
	}

	private static string CreateLogFilename()
	{
		var now = DateTime.Now;
		return Path.Combine(Configuration.MyHome, $"{now:yyyy-MM-dd} {$"{now.ToOADate():00000.0000000000}".Replace('.', '-')} - update.log");
	}

	private static string PairedLogMessage(string src, string dst) => PairedLogMessageDetail(src, dst, "SOURCE", "DESTIN");

	private static string PairedLogMessageDetail(string src, string dst, string srcName, string dstName) => $"\n\n {srcName}: {src}\n {dstName}: {dst}\n";

	private static void Log(string message)
	{
		var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}";
		File.AppendAllText(Configuration.TheLog, logMessage + Environment.NewLine);
	}

	private static void LogError(string message)
	{
		var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}";
		File.AppendAllText(Configuration.TheLog, logMessage + Environment.NewLine);
	}
}

[JsonSerializable(typeof(Evolver.EvolverContext))]
[JsonSerializable(typeof(Evolver.EvolverContext.ClientContext))]
public partial class EvolverJsonContext : JsonSerializerContext;
