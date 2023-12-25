﻿using Avalonia;
using System;

namespace SLA_Remake;

internal class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things
	// aren't initialized yet, and stuff might break. Proceed with caution.
	[STAThread]
	public static void Main(string[] args)
	{
		try
		{
			System.Net.ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
			BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
		}
		catch (Exception x)
		{
			WebAPI.RegisterException(x, verbose: true);

			System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(15)).Wait();
			System.Diagnostics.Process.Start(Environment.ProcessPath!);
		}
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<Host>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}