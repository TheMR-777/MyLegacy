using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SLA_Remake;

public class App : Application
{
	public static readonly SingleGlobalInstance Lock = new("h1m4a4a0h30-u3m5d1m5k2n-m0a4a0m0r1a");

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.ShutdownRequested += MainWindow.WindowClosing;
			desktop.Exit += MainWindow.WindowClosed;

			desktop.MainWindow = 
				!Lock.TryAcquireExclusiveLock() ? MakeAlternateWindow("SLA is already running", "SLA is already running. Only one instance of SLA can be running at a time.") :
				!WebAPI.VerifyVersion() ? MakeAlternateWindow("Outdated SLA", "This version of SLA is outdated. Please contact the NSS/ASC Department for the newer version.") :
				new MainWindow();
		}

		base.OnFrameworkInitializationCompleted();
	}

	// Utilities
	// ---------

	private static Avalonia.Controls.Window MakeAlternateWindow(string title, string content) => new()
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
		private System.IO.FileStream _lockFile;
		private readonly string _applicationID;

		public SingleGlobalInstance(string appID) => _applicationID = appID;

		public bool TryAcquireExclusiveLock()
		{
			var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), _applicationID);
			try
			{
				_lockFile = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void Dispose() => _lockFile?.Dispose();
	}
}