using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SLA_Remake;

public class Host : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			if (Grant.EveryAccessAcquired)
			{
				desktop.ShutdownRequested += MainWindow.WindowClosing;
				desktop.Exit += MainWindow.WindowClosed;
			}

			desktop.MainWindow = 
				Grant.DuplicateInstance ? AlternateWindow("Duplicate SLA Found", "Another instance of SLA is already running. Only one version of SLA can be running at a time.") :
				Grant.NewVersionRelease ? AlternateWindow("Newer SLA Available", "This version of SLA is outdated. Please contact the NSS/ASC Department for the newer version.") :
				new MainWindow();
		}

		base.OnFrameworkInitializationCompleted();
	}

	// Utilities
	// ---------

	public static readonly SingleGlobalInstance Lock = new();

	private static class Grant
	{
		public static bool EveryAccessAcquired => DuplicateInstance && NewVersionRelease;

		public static bool DuplicateInstance = !Lock.TryAcquireExclusiveLock();
		public static bool NewVersionRelease = !WebAPI.VerifyVersion();
	}

	private static Avalonia.Controls.Window AlternateWindow(string title, string content) => new()
	{
		Width = 400,
		Height = 200,
		WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen,
		Background = Avalonia.Media.Brush.Parse("#6E0D25"),
		Title = title,
		Content = new Avalonia.Controls.SelectableTextBlock
		{
			FontSize = 15,
			Opacity = 0.8,
			LineHeight = 20,
			Margin = new Thickness(10),
			Padding = new Thickness(10),
			FontWeight = Avalonia.Media.FontWeight.Bold,
			Foreground = Avalonia.Media.Brushes.White,
			SelectionBrush = Avalonia.Media.Brush.Parse("#FFFFB3"),
			Text = content,
			TextWrapping = Avalonia.Media.TextWrapping.Wrap,
			TextAlignment = Avalonia.Media.TextAlignment.Center,
			VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
			HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
		}
	};

	public class SingleGlobalInstance : IDisposable
	{
		// This class is using Singleton Pattern, with Specialized
		// keys and locks to ensure only one instance of SLA runs.

		private System.IO.FileStream _lockFile;
		const string ApplicationCognition = "h1m4a4a0h30-u3m5d1m5k2n-m0a4a0m0r1a";
		const string LockEnforcingLiteral = "K0MIVRVRBRAfWkRFVX8iYyEdTWUqZidoI3MkGEFgKGIhdDctYyx6InFNEAldVA09fCN6NhEPTEtyOkUIfTMZVgda";

		public bool TryAcquireExclusiveLock()
		{
			var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), ApplicationCognition);
			try
			{
				_lockFile = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
				var key = Convert.FromBase64String(LockEnforcingLiteral).Select((b, i) => (byte)(b ^ ApplicationCognition[i % ApplicationCognition.Length])).ToArray();
				_lockFile.Write(key, 0, key.Length); _lockFile.Flush();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Dispose() => _lockFile?.Dispose();
	}
}