﻿using Avalonia;
using System;
using System.Reflection;

namespace SLA_Remake;

internal class Program
{
	private static readonly System.Threading.Mutex mutex = new(true, Assembly.GetExecutingAssembly().FullName);

	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
	{
		try
		{
			if (!mutex.WaitOne(TimeSpan.Zero, true))
				return;

			BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
		}
		catch (Exception e)
		{
			if (Constants.EnableDiscordReporting)
				WebAPI.L1_RegisterException(e);
		}
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}