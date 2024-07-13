using System;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace TheProgram
{
	public class RSAEncryptionBouncyCastle
	{
		private static readonly SecureRandom Random = new();

		public static AsymmetricKeyParameter LoadPublicKey(string certificate, bool isFilePath = false)
		{
            var rsa = RSAEncryption.LoadPublicKey(certificate, isFilePath);
            return DotNetUtilities.GetRsaPublicKey(rsa);
        }

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
		public static RSAParameters LoadPublicKey(string certificate, bool isFilePath = false)
		{
			var cert = isFilePath
                ? new X509Certificate2(certificate)
                : new X509Certificate2(Convert.FromBase64String(certificate));
			var rsa = cert.GetRSAPublicKey()!.ExportParameters(includePrivateParameters: false);
			return rsa;
		}

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
		private static readonly System.Diagnostics.Stopwatch stopwatch = new();
		private static TimeSpan Elapsed => stopwatch.Elapsed;
		private static readonly string keyPath = @"D:\PNB Remittance\works_cert.txt";

		public static void Main()
		{
			while (true)
			{
				// Load public key from certificate
				var rawKey = File.ReadAllText(keyPath);
				var rsaKey = RSAEncryption.LoadPublicKey(rawKey);
				var bocKey = RSAEncryptionBouncyCastle.LoadPublicKey(rawKey);

				// Key Size Extraction
				var keySize = rsaKey.Modulus!.Length * 8;
				Console.WriteLine("Key Size : " + keySize + " bits");

				// Get Plain Text
				Console.WriteLine();
				Console.Write("Enter Text to Encrypt : ");
				var plainText = Console.ReadLine()!;

				// Encrypt, with C# .NET
				stopwatch.Restart();
				var netCipherText = RSAEncryption.SecureRSA(
					encrypt: true,
					source: plainText,
					key: rsaKey
				);
				stopwatch.Stop();
				Console.WriteLine();
				Console.WriteLine(".NET — Encrypted in   : " + Elapsed.TotalSeconds + " seconds");
				Console.WriteLine(".NET — Encrypted Text : " + netCipherText);

				// Encrypt, with Bouncy Castle
				stopwatch.Restart();
				var bocCipherText = RSAEncryptionBouncyCastle.SecureRSA(
					encrypt: true,
					source: plainText,
					key: bocKey
				);
				stopwatch.Stop();
				Console.WriteLine();
				Console.WriteLine("BOC  — Encrypted in    : " + Elapsed.TotalSeconds + " seconds");
				Console.WriteLine("BOC  — Encrypted Text  : " + bocCipherText);

				// Wait for user input
				Console.WriteLine();
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
				Console.Clear();
			}
		}
	}
}
