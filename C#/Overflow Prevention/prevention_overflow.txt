Console.WriteLine("Hello C# World");

// Unchecked
Byte x = 255;
Byte y = x++;

Console.WriteLine($"x = {x}, y = {y}");

// Checked
try
{
    checked
    {
        Byte z = 255;
        z++;
        Console.WriteLine($"z = {z}");
    }
}
catch (OverflowException o)
{
    Console.WriteLine(o.Message);
}
