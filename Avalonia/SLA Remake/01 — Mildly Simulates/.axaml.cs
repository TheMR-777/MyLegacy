using System;
using System.Timers;

class Program
{
    private static DateTime openTime;
    private static DateTime exitTime;

    static void Main()
    {
        // Create a Timer object that calls our TimerCallback method every 1000 milliseconds.
        Timer timer = new Timer(1 * 1000);

        openTime = DateTime.Now;

        // Hook up the Elapsed event for the timer.
        timer.Elapsed += OnTimedEvent;

        timer.Start();

        // Run long operation you want to measure, for simplicity run for 10 seconds here.
        System.Threading.Thread.Sleep(10000);

        timer.Stop();
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        exitTime = DateTime.Now - openTime;
        Console.WriteLine("Time elapsed: {0} seconds", elapsedSeconds.TotalSeconds);
    }
}
