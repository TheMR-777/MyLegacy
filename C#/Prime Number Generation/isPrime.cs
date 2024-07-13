
static bool isPrime(int Target)
{
    for (var x = (int)Math.Sqrt(Target); x > 1; --x)
    {
        if (Target % x == 0)
            return false;
    }
    return true;
}

Enumerable.Range(1, 50).Where(isPrime).ToList().ForEach(Console.WriteLine);
