using System.Diagnostics;
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
			LogError($"Error occurred in Preparation: {x.Message}");
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
			LogError($"Error occurred in Upgradation: {x.Message}");
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
			LogError($"Error occurred in Disposal: {x.Message}");
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
		catch (FormatException ex)
		{
			LogError($"Failed to Decode Base64: {ex.Message}");
			throw new ArgumentException("Invalid Base64 context.", ex);
		}

		EvolverContext context;
		try
		{
			context = JsonSerializer.Deserialize(jsonContext, EvolverJsonContext.Default.EvolverContext)!;
		}
		catch (JsonException ex)
		{
			LogError($"Failed to deserialize JSON context: {ex.Message}");
			throw new ArgumentException("Invalid JSON context.", ex);
		}

		ValidateContext(context);
		return context;
	}

	private static void ValidateContext(EvolverContext context)
	{
		const string message = "Invalid value for: ";

		if (string.IsNullOrEmpty(context.PackagePath) || !Directory.Exists(context.PackagePath))
			throw new ArgumentException(message + "package_path: " + context.PackagePath);

		if (context.BackupsKeep <= 0)
			throw new ArgumentException(message + "backups_keep. Backup Management is required");

		if (string.IsNullOrEmpty(context.Client.Destination) || !Directory.Exists(context.Client.Destination))
			throw new ArgumentException(message + "client_context.destination: " + context.Client.Destination);

		if (string.IsNullOrEmpty(context.Client.Executable))
			throw new ArgumentException(message + "client_context.executable.");

		if (context.Client.Restoration == null || context.Client.Restoration.Count == 0)
			throw new ArgumentException(message + "client_context.restoration. Client's state restoration is required.");
	}

	private static void TerminateTheClientApp()
	{
		try
		{
			Log("Trying to Terminate: " + Configuration.context.Client.Executable);
			var processes = Process.GetProcesses()
				.Where(p => p.ProcessName.Contains(Path.GetFileNameWithoutExtension(Configuration.context.Client.Executable), StringComparison.OrdinalIgnoreCase))
				.ToList();

			foreach (var process in processes)
			{
				Log("Terminating: " + process.ProcessName);
				process.Kill();
			}
		}
		catch (Exception ex)
		{
			LogError($"Failed to terminate client app: {ex.Message}");
			throw new ApplicationException("Could not terminate client app.", ex);
		}
	}

	private static string BackupCurrentVersion()
	{
		try
		{
			var source = Configuration.context.Client.Destination;
			var backup = Path.Combine(Configuration.Parent, Configuration.BackupFolder, $"{Path.GetFileName(source)} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}");
			Directory.CreateDirectory(backup);
			foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
			{
				var relative = file.Replace(source, string.Empty).TrimStart(Path.DirectorySeparatorChar);
				var destinationPath = Path.Combine(backup, relative);
				Copy(file, destinationPath);
			}
			Log("Current version backed up.");

			return backup;
		}
		catch (Exception ex)
		{
			LogError($"Failed to backup current version: {ex.Message}");
			throw new IOException("Backup process failed.", ex);
		}
	}

	private static void MoveUpdateContent()
	{
		var src = Path.Combine(Configuration.context.PackagePath, Path.GetFileNameWithoutExtension(Configuration.context.Client.Destination));
		var dst = Configuration.context.Client.Destination;
		try
		{
			CopyDirectory(src, dst);
			Log("Update content moved to installation folder.");
		}
		catch (Exception ex)
		{
			LogError($"Failed to move update content: {ex.Message}");
			throw new IOException("Move update content process failed.", ex);
		}
	}

	private static void RestoreClientContext()
	{
		try
		{
			foreach (var item in Configuration.context.Client.Restoration)
			{
				var backupPath = Path.Combine(Configuration.BackupDirectory, item);
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
					LogError($"The backup not found\n OF: {item}\n IN: {Configuration.BackupDirectory}");
				}
			}
		}
		catch (Exception ex)
		{
			LogError($"Failed to restore client context: {ex.Message}");
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
				Log($"Old backup deleted: {backupDirs[i]}");
			}

			// Manage update logs
			var logFiles = Directory.GetFiles(Configuration.MyHome, "*update.log")
				.OrderDescending()
				.ToList();

			for (var i = keep; i < logFiles.Count; i++)
			{
				File.Delete(logFiles[i]);
				Log($"Old update log deleted: {logFiles[i]}");
			}
		}
		catch (Exception ex)
		{
			LogError($"Failed to clean up old backups or logs: {ex.Message}");
			throw;
		}
	}

	private static void InitiateTheClientApp()
	{
		try
		{
			var details = new ProcessStartInfo
			{
				FileName = Path.Combine(Configuration.context.Client.Destination, Configuration.context.Client.Executable),
				Arguments = Configuration.context.Client.Arguments,
				UseShellExecute = true,
				CreateNoWindow = true,
			};
			Process.Start(details);
			Log($"Client app started with: {Configuration.context.Client.Arguments}");
		}
		catch (Exception ex)
		{
			LogError($"Failed to start client app: {ex.Message}");
			throw new ApplicationException("Could not start client app.", ex);
		}
	}

	private static void DisposeTheUpdate()
	{
		try
		{
			var path = Configuration.context.PackagePath;
			Directory.Delete(path, true);
			Log("The update package is deleted.");
		}
		catch (Exception ex)
		{
			LogError($"Failed to delete the update package: {ex.Message}");
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

			Log("Previous version restored successfully.");
		}
		catch (Exception ex)
		{
			LogError($"Failed to restore previous version: {ex.Message}");
		}
	}

	// Utilities

	private static void CopyDirectory(string source, string destin)
	{
		var message = $"\n\n SOURCE: {source}\n DESTIN: {destin}\n";
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
		var message = $"\n\n SOURCE: {source}\n DESTIN: {destination}\n";
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			File.Copy(source, destination, true);
			Log("File copied successfully" + message);
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
