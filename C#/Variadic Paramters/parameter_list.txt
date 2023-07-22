static int add(params int[] numbers)
{
    int sum = 0;
    foreach (int number in numbers)
    {
        sum += number;
    }
    return sum;
}

int x = 77, y = 47;
Console.WriteLine($"add({x}, {y}) = {add(x, y)}");