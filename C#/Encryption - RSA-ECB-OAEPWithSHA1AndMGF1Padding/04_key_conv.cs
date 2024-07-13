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

	public class ConvertKey
	{
		/// <summary>
		/// Converts <see cref="RSAParameters"/> to an <see cref="AsymmetricCipherKeyPair"/>.
		/// </summary>
		/// <remarks>
		/// If the private key is not present in <see cref="RSAParameters"/>, the <see cref="AsymmetricCipherKeyPair.Private"/> property will be null. <br></br>
		/// Which Means, the returned <see cref="AsymmetricCipherKeyPair"/> will be a public key only.
		/// </remarks>
		/// <param name="rsaParams"> <see cref="RSAParameters"/> to convert. </param>
		/// <returns> <see cref="AsymmetricCipherKeyPair"/> </returns>
		public static AsymmetricCipherKeyPair ToBouncyCastleKeys(RSAParameters rsaParams)
		{
			var modulus = new BigInteger(1, rsaParams.Modulus);
			var pubExpo = new BigInteger(1, rsaParams.Exponent);
			var pbParam = new RsaKeyParameters(
				isPrivate: false,
				modulus: modulus,
				exponent: pubExpo
			);

			return rsaParams.D == null
				? new AsymmetricCipherKeyPair(
					publicParameter: pbParam,
					privateParameter: null
				)
				: new AsymmetricCipherKeyPair(
					publicParameter: pbParam,
					privateParameter: new RsaPrivateCrtKeyParameters(
						modulus: modulus,
						publicExponent: pubExpo,
						privateExponent: new BigInteger(1, rsaParams.D),
						new BigInteger(1, rsaParams.P),
						new BigInteger(1, rsaParams.Q),
						new BigInteger(1, rsaParams.DP),
						new BigInteger(1, rsaParams.DQ),
						new BigInteger(1, rsaParams.InverseQ)
					)
				);
		}
		
		/// <summary>
		/// Converts <see cref="AsymmetricCipherKeyPair"/> to <see cref="RSAParameters"/>.
		/// </summary>
		/// <remarks>
		/// If the private key is not present in <see cref="AsymmetricCipherKeyPair"/>, the <see cref="RSAParameters.D"/>, and similar properties will be null. <br></br>
		/// Which means, the returned <see cref="RSAParameters"/> will be a public key only.
		/// </remarks>
		/// <param name="keys"> <see cref="AsymmetricCipherKeyPair"/> to convert. </param>
		/// <returns> <see cref="RSAParameters"/> </returns>
		/// <exception cref="ArgumentException"></exception>
		public static RSAParameters ToRSAParameters(AsymmetricCipherKeyPair keys)
		{
			if (keys.Public is not RsaKeyParameters pubKey)
				throw new ArgumentException("Invalid key type. Only RSA keys are supported.");

			var rsaParams = new RSAParameters
			{
				Modulus = pubKey.Modulus.ToByteArrayUnsigned(),
				Exponent = pubKey.Exponent.ToByteArrayUnsigned()
			};

			if (keys.Private is not RsaPrivateCrtKeyParameters privateParams)
				return rsaParams;

			rsaParams.D = privateParams.Exponent.ToByteArrayUnsigned();
			rsaParams.P = privateParams.P.ToByteArrayUnsigned();
			rsaParams.Q = privateParams.Q.ToByteArrayUnsigned();
			rsaParams.DP = privateParams.DP.ToByteArrayUnsigned();
			rsaParams.DQ = privateParams.DQ.ToByteArrayUnsigned();
			rsaParams.InverseQ = privateParams.QInv.ToByteArrayUnsigned();

			return rsaParams;
		}
	}

	public class Demo
	{
		private const int keySizeInBits = 2048;
		private static readonly System.Diagnostics.Stopwatch stopwatch = new();
		private static TimeSpan Elapsed => stopwatch.Elapsed;

		public static void Main()
		{
			while (true)
			{
				// Introduction
				Console.WriteLine("RSA Encryption, and Decryption");
				Console.WriteLine("with BouncyCastle, and C# .NET");
				Console.WriteLine("------------------------------");
				Console.WriteLine();

				// Generate Key Pair, with BouncyCastle
				//stopwatch.Restart();
				//var bocKey = RSAEncryptionBouncyCastle.GenerateKeyPair(keySizeInBits);
				//var rsaKey = ConvertKey.ToRSAParameters(bocKey);
				////var rsaKey = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)bocKey.Private);
				//stopwatch.Stop();
				//Console.WriteLine(" BC  — Key Pair Generated in: " + Elapsed.TotalSeconds + " seconds");

				// Generate Key Pair, with .NET
				stopwatch.Restart();
				var rsaKey = RSAEncryption.GenerateKeyPair(keySizeInBits);
				var bocKey = ConvertKey.ToBouncyCastleKeys(rsaKey);
				stopwatch.Stop();
				Console.WriteLine(".NET — Key Pair Generated in: " + Elapsed.TotalSeconds + " seconds");

				// Get Plain Text
				Console.WriteLine();
				Console.Write("Enter Text to Encrypt : ");
				var plainText = Console.ReadLine()!;

				// Encrypt, with BouncyCastle
				stopwatch.Restart();
				var bocCipherText = RSAEncryptionBouncyCastle.SecureRSA(
					encrypt: true,
					source: plainText,
					key: bocKey.Public
				);
				stopwatch.Stop();
				Console.WriteLine();
				Console.WriteLine(" BC  — Encrypted in   : " + Elapsed.TotalSeconds + " seconds");
				Console.WriteLine(" BC  — Encrypted Text : " + bocCipherText);

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

				// Decrypt, with BouncyCastle
				stopwatch.Restart();
				var bocDecryptedText = RSAEncryptionBouncyCastle.SecureRSA(
					encrypt: false,
					source: bocCipherText,
					key: bocKey.Private
				);
				stopwatch.Stop();
				Console.WriteLine();
				Console.WriteLine(" BC  — Decrypted in   : " + Elapsed.TotalSeconds + " seconds");
				Console.WriteLine(" BC  — Decrypted Text : " + bocDecryptedText);

				// Decrypt, with C# .NET
				stopwatch.Restart();
				var netDecryptedText = RSAEncryption.SecureRSA(
					encrypt: false,
					source: netCipherText,
					key: rsaKey
				);
				stopwatch.Stop();
				Console.WriteLine();
				Console.WriteLine(".NET — Decrypted in   : " + Elapsed.TotalSeconds + " seconds");
				Console.WriteLine(".NET — Decrypted Text : " + netDecryptedText);

				// Wait for user input
				Console.WriteLine();
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
				Console.Clear();
			}
		}
	}
}
