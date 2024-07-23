
namespace MyPlayground_C_;

public static class Program
{
    private const int min = 2, max = 6, limit = 20;

    public static void Main()
    {
        var runners = Enumerable.Range(1, limit).Select(_ => Runner().ContinueWith(task => Console.WriteLine($"Duration: {task.Result}")));
        Task.WaitAll(runners.ToArray());
    }

    private static async Task<int> Runner()
    {
        var duration = Random.Shared.Next(min, max);
        await Task.Delay(duration * 1000);
        return duration;
    }
}
