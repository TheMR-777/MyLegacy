
/* Base64 Encoding Demo */

while (true)
{
    Console.WriteLine("Base64 Encoding Demo");
    Console.WriteLine("--------------------");
    Console.WriteLine();

    // Take a String from the user
    Console.Write("Enter a String: ");
    string input = Console.ReadLine()!;

    // Base64 Encode the String
    var bytes = System.Text.Encoding.UTF8.GetBytes(input);
    var encoded = Convert.ToBase64String(bytes);

    // Display the Encoded String
    Console.WriteLine();
    Console.WriteLine("Encoded String: " + encoded);

    // Base64 Decode the String
    var decoded = Convert.FromBase64String(encoded);
    var output = System.Text.Encoding.UTF8.GetString(decoded);

    // Display the Decoded String
    Console.WriteLine("Decoded String: " + output);

    // Display the Relevant ASCII Values, with Binary
    Console.WriteLine();
    Console.WriteLine("ASCII Values: ");
    foreach (var c in input)
    {
        Console.WriteLine(c + ": " + ((int)c).ToString().PadLeft(3, ' ') + " (" + Convert.ToString(c, 2).PadLeft(8, '0') + ")");
    }

    // Wait for user to acknowledge the results
    Console.WriteLine();
    Console.WriteLine("Press Enter to terminate...");
    Console.ReadLine();
    
    // Clear the screen
    Console.Clear();
}
