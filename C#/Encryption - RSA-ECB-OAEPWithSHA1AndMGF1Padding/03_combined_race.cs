using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace TheProgram
{
    public class RSAEncryptionBouncyCastle
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

            var rawSource = encrypt
                ? Encoding.UTF8.GetBytes(source)        // Treats source as plain text
                : Convert.FromBase64String(source);     // Treats source as cipher text
            var rawBuffer = engine.ProcessBlock(
                inBytes: rawSource,
                inLen: rawSource.Length,
                inOff: 0
            );

            return !encrypt
                ? Encoding.UTF8.GetString(rawBuffer)    // Returns plain text
                : Convert.ToBase64String(rawBuffer);    // Returns cipher text
        }
    }

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
    }

    public class Demo
    {
        public static void Main()
        {
            const int keySizeInBits = 2048;
            var time_i = new DateTime();
            var time_f = new DateTime();

            while (true)
            {
                // Introduction
                Console.WriteLine("RSA Encryption, and Decryption");
                Console.WriteLine("with BouncyCastle, and C# .NET");
                Console.WriteLine("------------------------------");
                Console.WriteLine();

                // Generate Key Pair, with BouncyCastle
                time_i = DateTime.Now;
                var keyPair = RSAEncryptionBouncyCastle.GenerateKeyPair(keySizeInBits);
                var publicKey = keyPair.Public;
                var privateKey = keyPair.Private;
                time_f = DateTime.Now;
                Console.WriteLine(" BC  — Key Pair Generated in: " + (time_f - time_i).TotalSeconds + " seconds");

                // Generate Key Pair, with C# .NET
                time_i = DateTime.Now;
                var keys = RSAEncryption.GenerateKeyPair(keySizeInBits);
                time_f = DateTime.Now;
                Console.WriteLine(".NET — Key Pair Generated in: " + (time_f - time_i).TotalSeconds + " seconds");

                // Get Plain Text
                Console.WriteLine();
                Console.Write("Enter Text to Encrypt : ");
                var plainText = Console.ReadLine()!;

                // Encrypt, with BouncyCastle
                time_i = DateTime.Now;
                var bocCipherText = RSAEncryptionBouncyCastle.SecureRSA(
                    encrypt: true,
                    source: plainText,
                    key: publicKey
                );
                time_f = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(" BC  — Encrypted in   : " + (time_f - time_i).TotalSeconds + " seconds");
                Console.WriteLine(" BC  — Encrypted Text : " + bocCipherText);

                // Encrypt, with C# .NET
                time_i = DateTime.Now;
                var netCipherText = RSAEncryption.SecureRSA(
                    encrypt: true,
                    source: plainText,
                    key: keys
                );
                time_f = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(".NET — Encrypted in   : " + (time_f - time_i).TotalSeconds + " seconds");
                Console.WriteLine(".NET — Encrypted Text : " + netCipherText);

                // Decrypt, with BouncyCastle
                time_i = DateTime.Now;
                var decryptedText = RSAEncryptionBouncyCastle.SecureRSA(
                    encrypt: false,
                    source: bocCipherText,
                    key: privateKey
                );
                time_f = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(" BC  — Decrypted in   : " + (time_f - time_i).TotalSeconds + " seconds");
                Console.WriteLine(" BC  — Decrypted Text : " + decryptedText);

                // Decrypt, with C# .NET
                time_i = DateTime.Now;
                var decryptedText2 = RSAEncryption.SecureRSA(
                    encrypt: false,
                    source: netCipherText,
                    key: keys
                );
                time_f = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(".NET — Decrypted in   : " + (time_f - time_i).TotalSeconds + " seconds");
                Console.WriteLine(".NET — Decrypted Text : " + decryptedText2);

                // Wait for user input
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
