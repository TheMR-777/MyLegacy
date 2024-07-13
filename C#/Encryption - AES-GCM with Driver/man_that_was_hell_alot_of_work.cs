using System;
using System.Security.Cryptography;
using System.Text;

public class AesGcmEncryption
{
    /// <summary>
    /// Encrypts the given string using the final key (as described in the official documentation).
    /// </summary>
    /// <param name="rawData">
    /// Raw string to encrypt
    /// </param>
    /// <param name="finalKey">
    /// Final key to use for encryption (key|nonce)
    /// </param>
    /// <returns>
    /// Pair of strings: (auth-code, encrypted-data)
    /// </returns>
    public static (string, string) Encrypt(string rawData, string finalKey)
    {
        var keyAndNonce = finalKey.Split('|');
        var key = Convert.FromBase64String(keyAndNonce.First());
        var nonce = Convert.FromBase64String(keyAndNonce.Last());

        using var aesGcm = new AesGcm(key);
        var plaintextBytes = Encoding.UTF8.GetBytes(rawData);
        var cipherText = new byte[plaintextBytes.Length];
        var tag = new byte[16];

        aesGcm.Encrypt(nonce, plaintextBytes, cipherText, tag);

        return (Convert.ToBase64String(tag), Convert.ToBase64String(cipherText));
    }

    /// <summary>
    /// Decrypts the given string using the final key, and auth-code (as described in the official documentation).
    /// </summary>
    /// <param name="encryptedData">
    /// Encrypted string
    /// </param>
    /// <param name="finalKey">
    /// Final key to use for decryption (key|nonce)
    /// </param>
    /// <param name="tag">
    /// Authentication Code (tag) to use for decryption
    /// </param>
    /// <returns>
    /// Decrypted string
    /// </returns>
    public static string Decrypt(string encryptedData, string finalKey, string tag)
    {
        var keyAndNonce = finalKey.Split('|');
        var key = Convert.FromBase64String(keyAndNonce.First());
        var nonce = Convert.FromBase64String(keyAndNonce.Last());

        using var aesGcm = new AesGcm(key);
        var ciphertextBytes = Convert.FromBase64String(encryptedData);
        var tagBytes = Convert.FromBase64String(tag);
        var decryptedData = new byte[ciphertextBytes.Length];

        aesGcm.Decrypt(nonce, ciphertextBytes, tagBytes, decryptedData);

        return Encoding.UTF8.GetString(decryptedData);
    }
}

class Program
{
    static void Main()
    {
        var Print = new Action<string>(Console.WriteLine);

        while (true)
        {
            Print("AES-GCM Encryption and Decryption");
            Print("---------------------------------");

            byte[] key = RandomNumberGenerator.GetBytes(32);    // 256-bit key
            byte[] nonce = RandomNumberGenerator.GetBytes(12);  // 96-bit nonce

            string finalKey = $"{Convert.ToBase64String(key)}|{Convert.ToBase64String(nonce)}";
            Print($"Selected Final-Key    : {finalKey}\n");

            Console.Write("Enter text to encrypt : ");
            string rawData = Console.ReadLine()!;

            var (tag, encryptedData) = AesGcmEncryption.Encrypt(rawData, finalKey);
            Print("");
            Print($"Encrypted Text        : {encryptedData}");
            Print($"Authentication Code   : {tag}\n");

            string decryptedData = AesGcmEncryption.Decrypt(encryptedData, finalKey, tag);
            Print($"Decrypted Text        : {decryptedData}\n");

            Print("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
