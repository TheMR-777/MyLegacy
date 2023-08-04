
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.OpenSsl;

namespace MySpace
{
	class Utility
	{
		public static string GetSHA256(string input)
		{
			var hashedBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}

		public class AES_GCM
		{
			public static string GenerateFinalKey(int keyLength = 256, int nonceSize = 128)
			{
				try
				{
					var enKey = new byte[keyLength / 8];
					var nonce = new byte[nonceSize / 8];

					using (var rng = RandomNumberGenerator.Create())
					{
						rng.GetBytes(enKey);
						rng.GetBytes(nonce);
					}

					return $"{Convert.ToBase64String(enKey)}|{Convert.ToBase64String(nonce)}";
				}
				catch
				{
					return string.Empty;
				}
			}

			public static string Apply(string source, string finalKey, bool toEncrypt = true, int macSize = 128)
			{
				try
				{
					var keyAndNonce = finalKey.Split('|');
					var key = Convert.FromBase64String(keyAndNonce.First());
					var nonce = Convert.FromBase64String(keyAndNonce.Last());

					var cipher = new GcmBlockCipher(new AesEngine());
					cipher.Init(
						forEncryption: toEncrypt,
						parameters: new AeadParameters(
							key: new KeyParameter(key),
							macSize: macSize,
							nonce: nonce
						)
					);

					var rawSource = toEncrypt
						? Encoding.UTF8.GetBytes(source)        // Treats as Text
						: Convert.FromBase64String(source);     // Treats as Cipher
					var rawBuffer = new byte[cipher.GetOutputSize(rawSource.Length)];

					cipher.DoFinal(
						output: rawBuffer,
						outOff: cipher.ProcessBytes(
							input: rawSource,
							output: rawBuffer,
							len: rawSource.Length,
							inOff: 0,
							outOff: 0
						)
					);

					return toEncrypt
						? Convert.ToBase64String(rawBuffer)     // Returns Cipher
						: Encoding.UTF8.GetString(rawBuffer);   // Returns Text
				}
				catch
				{
					return string.Empty;
				}
			}
		}

		public class RSA_ECB_OAEPwithSHA1andMGF1
		{
			public static RSAParameters LoadCertificate(string certificate, bool isFilePath = false, bool isPrivate = false)
			{
				var cert = isFilePath
					? new X509Certificate2(fileName: certificate)
					: new X509Certificate2(rawData: Convert.FromBase64String(certificate));
				var rsa = cert.GetRSAPublicKey().ExportParameters(includePrivateParameters: isPrivate);
				return rsa;
			}

			public static RSAParameters GenerateKeyPair(int keySizeInBits = 2048)
			{
				var rsa = new RSACryptoServiceProvider(keySizeInBits);
				return rsa.ExportParameters(includePrivateParameters: true);
			}

			public static string Apply(string source, RSAParameters key, bool toEncrypt = true)
			{
				var rsa = RSA.Create();
				rsa.ImportParameters(key);

				var rawSource = toEncrypt
					? Encoding.UTF8.GetBytes(source)
					: Convert.FromBase64String(source);
				var rawBuffer = toEncrypt
					? rsa.Encrypt(rawSource, RSAEncryptionPadding.OaepSHA1)
					: rsa.Decrypt(rawSource, RSAEncryptionPadding.OaepSHA1);

				return toEncrypt
					? Convert.ToBase64String(rawBuffer)
					: Encoding.UTF8.GetString(rawBuffer);
			}

			public static RSAParameters ImportPemKey(string keyPath, bool isPrivate = false)
			{
				using var rsa = RSA.Create();
				rsa.ImportFromPem(File.ReadAllText(keyPath));
				return rsa.ExportParameters(includePrivateParameters: isPrivate);
			}

			public static RSAParameters ImportPemKey_BC(string keyPath)
			{
				var rsaParameters = new RSAParameters();

				using var reader = new StreamReader(keyPath);
				var pemReader = new PemReader(reader);
				var keyObject = pemReader.ReadObject();

				if (keyObject is RsaPrivateCrtKeyParameters privateKey)
				{
					rsaParameters.Modulus = privateKey.Modulus.ToByteArrayUnsigned();
					rsaParameters.Exponent = privateKey.PublicExponent.ToByteArrayUnsigned();
					rsaParameters.D = privateKey.Exponent.ToByteArrayUnsigned();
					rsaParameters.P = privateKey.P.ToByteArrayUnsigned();
					rsaParameters.Q = privateKey.Q.ToByteArrayUnsigned();
					rsaParameters.DP = privateKey.DP.ToByteArrayUnsigned();
					rsaParameters.DQ = privateKey.DQ.ToByteArrayUnsigned();
					rsaParameters.InverseQ = privateKey.QInv.ToByteArrayUnsigned();
				}
				else if (keyObject is RsaKeyParameters publicKey)
				{
					rsaParameters.Modulus = publicKey.Modulus.ToByteArrayUnsigned();
					rsaParameters.Exponent = publicKey.Exponent.ToByteArrayUnsigned();
				}

				return rsaParameters;
			}
		}

		public class RequestModel
		{
			public static string GetBalance(string accountNumber) =>
				$"<GetAccountBalance xmlns:pnb=\"http://PNB_Inward_Remittance\" xmlns:tem=\"http://tempuri.org/\" xmlns=\"http://tempuri.org/\">" +
					$"<pnb:AccountNumber>{accountNumber}</pnb:AccountNumber>" +
				$"</GetAccountBalance>";
		}
	}

	class Demo
	{
		public static string accountNum = "455300VO00000628";
		public static string pubKeyPath = @"D:\PNB Remittance\Created Keypair\pub.pem";     // For Encryption
		public static string pvtKeyPath = @"D:\PNB Remittance\Created Keypair\pvt.pem";     // For Decryption

		// Format of Encryption:
		// ---------------------
		// 1. Key Generation
		//    - AES_GCM.GenerateFinalKey() -> finalKey
		//    - Load Public Key, from File specified
		//
		// 2. Encryption
		//    - RemitMetaData: Encrypt finalKey with Public Key (Certificate)
		//    - RemitMetaInfo: Encrypt the Data with finalKey, using AES_GCM
		//    - RemitMetaEntity: SHA256 Hash of the Account Number
		//
		// 3. Decryption
		//    - RemitMetaData: Decrypt the Encrypted finalKey with Private Key
		//    - RemitMetaInfo: Decrypt the Data with finalKey, using AES_GCM

		public static void Main()
		{
			// Introduction
			Console.WriteLine("PNB Remittance - Encryption/Decryption Demo");
			Console.WriteLine("-------------------------------------------");
			Console.WriteLine();

			// 1. Key Generation
			var finalKey = Utility.AES_GCM.GenerateFinalKey();
			var pubKey = Utility.RSA_ECB_OAEPwithSHA1andMGF1.ImportPemKey(pubKeyPath);
			var pvtKey = Utility.RSA_ECB_OAEPwithSHA1andMGF1.ImportPemKey(pvtKeyPath, isPrivate: true);

			Console.WriteLine("1. Key Generation");
			Console.WriteLine("------------------");
			Console.WriteLine($" - Final Key   : {finalKey}");
			Console.WriteLine();

			// 2. Encryption
			var remitMetaData = Utility.RSA_ECB_OAEPwithSHA1andMGF1.Apply(finalKey, pubKey);
			var remitMetaInfo = Utility.AES_GCM.Apply(Utility.RequestModel.GetBalance(accountNum), finalKey);
			var remitMetaEntity = Utility.GetSHA256(accountNum);

			Console.WriteLine("2. Encryption");
			Console.WriteLine("-------------");
			Console.WriteLine($" - RemitMetaEntity : {remitMetaEntity}");
			Console.WriteLine($" - RemitMetaInfo   : {remitMetaInfo}");
			Console.WriteLine($" - RemitMetaData   : {remitMetaData}");
			Console.WriteLine();

			// 3. Decryption
			var decrypted_finalKey = Utility.RSA_ECB_OAEPwithSHA1andMGF1.Apply(remitMetaData, pvtKey, toEncrypt: false);
			var decrypted_remitMetaInfo = Utility.AES_GCM.Apply(remitMetaInfo, decrypted_finalKey, toEncrypt: false);

			Console.WriteLine("3. Decryption");
			Console.WriteLine("-------------");
			Console.WriteLine($" - Decrypted Final Key     : {decrypted_finalKey}");
			Console.WriteLine($" - Decrypted RemitMetaInfo : {decrypted_remitMetaInfo}");
			Console.WriteLine();
		}
	}
}
