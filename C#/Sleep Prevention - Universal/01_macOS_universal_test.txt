
// Fields
// ------

using System.Diagnostics;

var _lastTime = DateTime.Now;
var _idleTime = TimeSpan.Zero;

while (true)
{
    BackgroundTimer_Tick();
    Thread.Sleep(1000);
}

//void BackgroundTimer_Tick()
//{
//    // Time Mitigation
//    // ---------------

//    Delay();
//    var current = DateTime.Now;
//    var elapsed = current - _lastTime;
//    _lastTime = current;

//    Console.WriteLine();
//    Console.WriteLine($"Last Time: {_lastTime:hh:mm:ss}");
//    Console.WriteLine($"Current Time: {current:hh:mm:ss}");
//    Console.WriteLine($"Elapsed Time: {elapsed.TotalSeconds} seconds");

//    // Idle Time Calculation
//    // ---------------------

//    _idleTime = TimeSpan.FromSeconds(25);
//    _idleTime += elapsed - TimeSpan.FromSeconds(1);

//    // Check for Trigger
//    // -----------------

//    if (_idleTime < TimeSpan.FromSeconds(30)) return;

//    Console.WriteLine();
//    Console.WriteLine(@"          + ------------ +          ");
//    Console.WriteLine(@" ._______ | SLA-TRIGGERS | _______. ");
//    Console.WriteLine(@" |_______ | SLA-TRIGGERS | _______| ");
//    Console.WriteLine(@"   |__|   + ------------ +   |__|   ");
//    Console.WriteLine(@"   /__\                      /__\   ");
//}

void BackgroundTimer_Tick()
{
    // Time Mitigation
    // ---------------

    var current = DateTime.Now;
    var elapsed = current - _lastTime;
    _lastTime = current;

    Console.WriteLine();
    Console.WriteLine($"Last Time: {_lastTime:hh:mm:ss}");
    Console.WriteLine($"Current Time: {current:hh:mm:ss}");
    Console.WriteLine($"Elapsed Time: {elapsed.TotalSeconds} seconds");

    // Idle Time Calculation
    // ---------------------

    _idleTime = GetIdleTime();

    Console.WriteLine($"[1/2] Idle Time: {_idleTime.TotalSeconds} seconds");

    _idleTime += elapsed - TimeSpan.FromSeconds(1);

    Console.WriteLine($"[2/2] Idle Time: {_idleTime.TotalSeconds} seconds");

    // Check for Trigger
    // -----------------

    if (_idleTime < TimeSpan.FromSeconds(60)) return;

    Console.WriteLine();
    Console.WriteLine(@"          + ------------ +          ");
    Console.WriteLine(@" ._______ | SLA-TRIGGERS | _______. ");
    Console.WriteLine(@" |_______ | SLA-TRIGGERS | _______| ");
    Console.WriteLine(@"   |__|   + ------------ +   |__|   ");
    Console.WriteLine(@"   /__\                      /__\   ");
}

void Delay(int seconds = 5)
{
    Thread.Sleep(seconds * 1000);
}

TimeSpan GetIdleTime()
{
    var detail = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        Arguments = "-c \"ioreg -c IOHIDSystem | awk '/HIDIdleTime/ {print $NF/1000000000; exit}'\""
    };

    using var process = Process.Start(detail);
    var output = process?.StandardOutput.ReadToEnd() ?? "0";
    process?.WaitForExit();

    var idleTimeSec = double.TryParse(output, out var result) ? result : 0;
    return TimeSpan.FromSeconds(idleTimeSec);
}
