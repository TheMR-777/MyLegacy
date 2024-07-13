using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Diagnostics;

namespace TheProgram;

public class RSAwithBouncyCastle
{
    private static readonly SecureRandom Random = new();

    public static AsymmetricCipherKeyPair GenerateKeyPair(int keySizeInBits = 2048)
    {
        var keyGenerationParameters = new KeyGenerationParameters(Random, keySizeInBits);
        var keyPairGenerator = new RsaKeyPairGenerator();
        keyPairGenerator.Init(keyGenerationParameters);
        return keyPairGenerator.GenerateKeyPair();
    }

    public static string Apply(string source, AsymmetricKeyParameter key, bool toEncrypt = true)
    {
        var engine = new OaepEncoding(
            new RsaEngine(),
            new Sha256Digest(),
            new Sha256Digest(),
            null
        );
        engine.Init(toEncrypt, key);

        var data = toEncrypt ? Encoding.UTF8.GetBytes(source) : Convert.FromBase64String(source);
        var result = engine.ProcessBlock(data, 0, data.Length);

        return toEncrypt ? Convert.ToBase64String(result) : Encoding.UTF8.GetString(result);
    }

    public static AsymmetricKeyParameter LoadPrivateKey(string privateKeyBase64)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        var privateKeyAsn1 = Org.BouncyCastle.Asn1.Asn1Object.FromByteArray(privateKeyBytes);
        var privateKeyStructure = PrivateKeyInfo.GetInstance(privateKeyAsn1);
        return PrivateKeyFactory.CreateKey(privateKeyStructure);
    }
}

public class RSAwithNET
{
    public static RSAParameters GenerateKeyPair(int keySizeInBits = 2048)
    {
        using var rsa = RSA.Create(keySizeInBits);
        return rsa.ExportParameters(includePrivateParameters: true);
    }

    public static string Apply(string source, RSAParameters key, bool toEncrypt = true)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(key);

        var data = toEncrypt ? Encoding.UTF8.GetBytes(source) : Convert.FromBase64String(source);
        var result = toEncrypt ? rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256) : rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);

        return toEncrypt ? Convert.ToBase64String(result) : Encoding.UTF8.GetString(result);
    }

    public static RSA LoadPrivateKey(string privateKeyBase64)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        return rsa;
    }
}

public class Demo
{
    private const int KeySize = 2048;
    private static readonly Stopwatch Stopwatch = new();
    private static string Elapsed => Stopwatch.Elapsed.TotalSeconds.ToString("0.00000");

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("RSA Encryption/Decryption Demonstration");
            Console.WriteLine("Using RSA with OAEP SHA-256 Padding");
            Console.WriteLine("—————————————————————————————————————————");
            Console.WriteLine();

            Stopwatch.Restart();
            var netKey = RSAwithNET.GenerateKeyPair(KeySize);
            Stopwatch.Stop();
            Console.WriteLine($"NET — Key Generation : {Elapsed}s");

            Stopwatch.Restart();
            var bocKey = RSAwithBouncyCastle.GenerateKeyPair(KeySize);
            Stopwatch.Stop();
            Console.WriteLine($"BOC — Key Generation : {Elapsed}s");

            Console.WriteLine();
            Console.Write("Enter the Plain Text : ");
            var input = Console.ReadLine() ?? "";

            Stopwatch.Restart();
            var netCipher = RSAwithNET.Apply(input, netKey, toEncrypt: true);
            Stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"NET — Encrypted in   : {Elapsed}s");
            Console.WriteLine($"NET — Encrypted Text : {netCipher}");

            Stopwatch.Restart();
            var bocCipher = RSAwithBouncyCastle.Apply(input, bocKey.Public, toEncrypt: true);
            Stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"BOC — Encrypted in   : {Elapsed}s");
            Console.WriteLine($"BOC — Encrypted Text : {bocCipher}");

            Stopwatch.Restart();
            var netPlain = RSAwithNET.Apply(netCipher, netKey, toEncrypt: false);
            Stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"NET — Decrypted in   : {Elapsed}s");
            Console.WriteLine($"NET — Decrypted Text : {netPlain}");

            Stopwatch.Restart();
            var bocPlain = RSAwithBouncyCastle.Apply(bocCipher, bocKey.Private, toEncrypt: false);
            Stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"BOC — Decrypted in   : {Elapsed}s");
            Console.WriteLine($"BOC — Decrypted Text : {bocPlain}");

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
