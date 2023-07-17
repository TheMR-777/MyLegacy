static string GetSHA256(string input)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(hashedBytes).Replace('-', (char)0x00).ToLower();
}

Console.WriteLine(GetSHA256("TheMR"));