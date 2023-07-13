for (int x = 0, y = 1; x < 1000; x = (y += x) - x)
{
    System.Console.Write(x.ToString() + " ");
}
