using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

public class RSAEncryption
{
    private static readonly SecureRandom Random = new();

    public static AsymmetricCipherKeyPair GenerateKeyPair(int keySizeInBits)
    {
        var keyParameters = new KeyGenerationParameters(Random, strength: keySizeInBits);
        var keysGenerator = new RsaKeyPairGenerator();
        keysGenerator.Init(keyParameters);
        return keysGenerator.GenerateKeyPair();
    }

    public static string SecureRSA(string source, AsymmetricKeyParameter key, bool encrypt)
    {
        var engine = new OaepEncoding(
            cipher: new RsaEngine(), 
            hash: new Sha1Digest(), 
            mgf1Hash: new Sha1Digest(), 
            encodingParams: null
        );
        engine.Init(encrypt, key);

        var rawSource = Encoding.UTF8.GetBytes(source);
        var rawBuffer = engine.ProcessBlock(
            inBytes: rawSource, 
            inLen: rawSource.Length,
            inOff: 0
        );

        return !encrypt 
            ? Encoding.UTF8.GetString(rawBuffer)    // Returns plain text
            : Convert.ToBase64String(rawBuffer);    // Returns cipher text
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
            var keyPair = GenerateKeyPair(2048);
            var publicKey = keyPair.Public;
            var privateKey = keyPair.Private;

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
