using System;
using System.Security.Cryptography;
using System.Text;

public class RSAEncryption
{
    public static RSAParameters GenerateKeyPair(int keySizeInBits)
    {
        using var rsa = new RSACryptoServiceProvider(keySizeInBits);
        return rsa.ExportParameters(includePrivateParameters: true);
    }

    public static string SecureRSA(string source, RSAParameters key, bool encrypt)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(key);

        var rawSource = encrypt 
            ? Encoding.UTF8.GetBytes(source) 
            : Convert.FromBase64String(source);
        var rawBuffer = encrypt
            ? rsa.Encrypt(rawSource, fOAEP: true)
            : rsa.Decrypt(rawSource, fOAEP: true);

        return encrypt 
            ? Convert.ToBase64String(rawBuffer) 
            : Encoding.UTF8.GetString(rawBuffer);
    }

    public static void Main()
    {
        while (true)
        {
            // Introduction
            Console.WriteLine("RSA Encryption, and Decryption");
            Console.WriteLine("------------------------------");

            // Generate Key Pair
            var initialTime = DateTime.Now;
            var keyPair = GenerateKeyPair(keySizeInBits: 2048);
            var publicKey = keyPair;
            var privateKey = keyPair;

            // Get Plain Text
            var finalTime = DateTime.Now;
            Console.WriteLine("Key Pair Generated in: " + (finalTime - initialTime).TotalSeconds + " seconds");
            Console.Write("Enter Text to Encrypt: ");
            var plainText = Console.ReadLine()!;

            // Encrypt
            var cipherText = SecureRSA(
                encrypt: true,
                source: plainText,
                key: publicKey
            );
            Console.WriteLine();
            Console.WriteLine("Encrypted Text: " + cipherText);

            // Decrypt
            var decryptedText = SecureRSA(
                encrypt: false,
                source: cipherText,
                key: privateKey
            );
            Console.WriteLine();
            Console.WriteLine("Decrypted Text: " + decryptedText);

            // Wait for user input
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
