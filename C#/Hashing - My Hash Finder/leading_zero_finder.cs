using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    public static string GetSHA256(string input)
    {
        var raw = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(raw).Replace("-", "").ToLower();
    }

    static void Main()
    {
        string input = "TheMR";
        string hash;
        int zeros;
        int maxZeros = 0;
        int count = 0;

        while (true)
        {
            hash = GetSHA256(input + count.ToString());
            zeros = 0;
            foreach (char c in hash)
            {
                if (c == '0')
                {
                    zeros++;
                }
                else
                {
                    break;
                }
            }
            if (zeros > maxZeros)
            {
                maxZeros = zeros;
                Console.WriteLine("Hash   : " + hash + " Zeros : " + zeros);
                Console.WriteLine("String : " + input + count.ToString());
            }
            count++;
        }
    }
}