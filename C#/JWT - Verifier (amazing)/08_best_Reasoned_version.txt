using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Project
{
	class Program
	{
		static readonly string jwt_src = @"D:\PayUK\jxt.txt";
		static readonly string req_aud = "0014H00003ARna3QAD";
		static readonly string key_id = "pVMomQ1NwB6nKuSAxaQDM0PrBg0";

		public static void Main()
		{
			var verifier = new JwtTokenVerifier(
				jwtSource: File.ReadAllText(jwt_src),
				requiredAudience: req_aud
			);

			var verificationResult = verifier.Verify();

			if (verificationResult.IsSuccess)
			{
				Console.WriteLine("JWT Verified Successfully");
			}
			else
			{
				Console.WriteLine("----- Failed to Verify JWT -----");
				Console.WriteLine($"Reason: {verificationResult.Reason}");
			}
		}
	}

	public class JwtTokenVerifier
	{
		private readonly string jwtSource;
		private JwtSecurityToken? jwt;
		private readonly string requiredAudience;
		private readonly string keyEndpoint;

		public JwtTokenVerifier(string jwtSource, string requiredAudience, string endpoint = "https://keystore.openbankingtest.org.uk/0014H00003ARnZ4QAL/0014H00003ARnZ4QAL.jwks")
		{
			// jwt = GetJwtToken(jwtSource);
			this.jwtSource = jwtSource;
			this.requiredAudience = requiredAudience;
			keyEndpoint = endpoint;
		}

		public VerificationResult Verify()
		{
			var jwtResult = GetJwtToken(jwtSource);
			if (!jwtResult.IsSuccess)
			{
                return jwtResult;
            }

			if (!VerifyExpiration())
			{
				return VerificationResult.Failed("JWT is Expired");
			}

			if (!VerifyAudience())
			{
				return VerificationResult.Failed($"JWT is not for this Audience. Required Audience: {requiredAudience}, Received Audience: {jwt!.Audiences.First()}");
			}

			var signatureResult = VerifySignature();
			if (!signatureResult.IsSuccess)
			{
				return VerificationResult.Failed($"JWT Signature is not Verified. \nReason: {signatureResult.Reason}");
			}

			return VerificationResult.Success();
		}

		private VerificationResult GetJwtToken(string source)
		{
			try
			{
                jwt = new JwtSecurityTokenHandler().ReadJwtToken(source);
				return VerificationResult.Success();
            }
			catch
			{
				return VerificationResult.Failed("Failed to Parse JWT Token");
			}
		}

		private string GetPublicKey(bool raw = true)
		{
			try
			{
                var json = new System.Net.WebClient().DownloadString(keyEndpoint);
                var jobj = System.Text.Json.JsonDocument.Parse(json);
                var keys = jobj.RootElement.GetProperty("keys");

                foreach (var key in keys.EnumerateArray())
                {
                    var extractedKid = key.GetProperty("kid").GetString();

                    if (string.IsNullOrEmpty(extractedKid) || extractedKid != jwt!.Header.Kid)
                    {
                        continue;
                    }

                    var x5c = key.GetProperty("x5c").EnumerateArray().First().GetString();

                    if (string.IsNullOrEmpty(x5c))
                    {
                        return string.Empty;
                    }
                    return raw ? x5c : $"-----BEGIN CERTIFICATE-----\n{x5c}\n-----END CERTIFICATE-----";
                }
                return string.Empty;
            }
			catch
			{
				 return string.Empty;
			}
		}

		private bool VerifyExpiration()
		{
			var now = DateTime.UtcNow;
			var iss = jwt!.ValidFrom;
			var exp = jwt!.ValidTo;
			return now >= iss && now <= exp;
		}

		private bool VerifyAudience()
		{
			return jwt!.Audiences.Select(a => a.Trim()).Contains(requiredAudience.Trim());
		}

		private VerificationResult VerifySignature()
		{
			try
			{
                var formatted = Encoding.UTF8.GetBytes($"{jwt!.RawHeader}.{jwt.RawPayload}");
                var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
                var publicKey = GetPublicKey();

                if (string.IsNullOrEmpty(publicKey))
                {
                    return VerificationResult.Failed("Public Key is not available");
                }

                var sigVerify = new X509Certificate2(Convert.FromBase64String(publicKey))
					.GetRSAPublicKey()!
					.VerifyData(
						data: formatted,
						signature: signature,
						hashAlgorithm: HashAlgorithmName.SHA256,
						padding: RSASignaturePadding.Pss
					);

				if (sigVerify)
				{
					return VerificationResult.Success();
				}
				else
				{
					return VerificationResult.Failed("Signature Verification Failed");
				}
			}
			catch (Exception ex)
			{
				return VerificationResult.Failed($"Signature Verification Failed with Exception: {ex.Message}");
			}
		}
	}

	public class VerificationResult
	{
		public bool IsSuccess = true;
		public string Reason = string.Empty;

		public static VerificationResult Failed(string reason)
		{
			return new VerificationResult
			{
				IsSuccess = false,
				Reason = reason
			};
		}

		public static VerificationResult Success()
		{
			return new VerificationResult();
        }
	}
}
