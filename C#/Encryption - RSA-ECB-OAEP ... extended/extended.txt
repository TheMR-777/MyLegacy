using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TheProgram
{
	public class RSAwithNET
	{
		public static RSAParameters LoadCertificate(string certificate, bool isFilePath = false)
		{
			var cert = isFilePath
				? new X509Certificate2(certificate)
				: new X509Certificate2(Convert.FromBase64String(certificate));
			return cert.GetRSAPublicKey()!.ExportParameters(includePrivateParameters: false);
		}

		public static RSAParameters GenerateKeyPair(int keySizeInBits = 256 * 8)
		{
			using var rsa = new RSACryptoServiceProvider(keySizeInBits);
			return rsa.ExportParameters(includePrivateParameters: true);
		}

		public static string Apply(string source, RSAParameters key, bool toEncrypt = true)
		{
			using var rsa = RSA.Create();
			rsa.ImportParameters(key);

			var rawSource = toEncrypt
				? Encoding.UTF8.GetBytes(source)
				: Convert.FromBase64String(source);
			var rawBuffer = toEncrypt
				? rsa.Encrypt(rawSource, RSAEncryptionPadding.OaepSHA1)
				: rsa.Decrypt(rawSource, RSAEncryptionPadding.OaepSHA1);

			return toEncrypt
				? Convert.ToBase64String(rawBuffer)     // Returns cipher
				: Encoding.UTF8.GetString(rawBuffer);   // Returns text
		}

		public static RSAParameters ImportKeyPair(string publicKeyPath, string privateKeyPath)
		{
			var publicKey = ImportKey(publicKeyPath);
			var privateKey = ImportKey(privateKeyPath, isPrivate: true);
			return new RSAParameters
			{
				Modulus = publicKey.Modulus,
				Exponent = publicKey.Exponent,
				D = privateKey.D,
				P = privateKey.P,
				Q = privateKey.Q,
				DP = privateKey.DP,
				DQ = privateKey.DQ,
				InverseQ = privateKey.InverseQ
			};
		}

		public static RSAParameters ImportKey(string keyPath, bool isPrivate = false)
		{
			using var rsa = RSA.Create();
			rsa.ImportFromPem(File.ReadAllText(keyPath));
			return rsa.ExportParameters(includePrivateParameters: isPrivate);
		}

		public static void ExportKeyPair(RSAParameters keyPair, string publicKeyFilePath, string privateKeyFilePath)
		{
			ExportKey(keyPair, publicKeyFilePath);
			ExportKey(keyPair, privateKeyFilePath, true);
		}

		public static void ExportKey(RSAParameters keyPair, string publicKeyFilePath, bool isPrivate = false)
		{
			using var rsa = RSA.Create();
			rsa.ImportParameters(keyPair);

			var key = isPrivate
				? rsa.ExportPkcs8PrivateKeyPem()
				: rsa.ExportSubjectPublicKeyInfoPem();

			File.WriteAllText(publicKeyFilePath, key);
		}
	}

	public class Demo
	{
		private const int keySize = 256 * 8;
		private static readonly string writeLoc = @"D:\PNB Remittance\Test Keys\";
		private static readonly string pubKeyPath = writeLoc + "pub.pem";
		private static readonly string pvtKeyPath = writeLoc + "pvt.pem";

		public static void Main()
		{
			var myData = "TheMR, from ASC";
            Console.WriteLine("Original : " + myData);
            Console.WriteLine();

            // Generating Key Pair
            var keyPair = RSAwithNET.GenerateKeyPair(keySize);

			// Exporting Key Pair
			RSAwithNET.ExportKeyPair(keyPair, pubKeyPath, pvtKeyPath);

			// Importing Key Pair
			keyPair = RSAwithNET.ImportKeyPair(pubKeyPath, pvtKeyPath);

			// Encryption
			var cipher = RSAwithNET.Apply(myData, keyPair);
			Console.WriteLine("Ciphered : " + cipher);
            Console.WriteLine();

			// URL Safe Version
			var raw = Convert.FromBase64String(cipher);
			var safe = Jose.Base64Url.Encode(raw);
			Console.WriteLine("URL Safe : " + safe);
            Console.WriteLine();

			// Decryption
			var text = RSAwithNET.Apply(cipher, keyPair, toEncrypt: false);
			Console.WriteLine("Decipher : " + text);
            Console.WriteLine();
        }
	}
}
