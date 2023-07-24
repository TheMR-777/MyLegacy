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
                Console.WriteLine("JWT Token Verified Successfully");
            }
            else
            {
                Console.WriteLine("----- Failed to Verify JWT Token -----");

                if (!verificationResult.IsExpirationValid)
                {
                    Console.WriteLine("Reason: JWT Token is Expired");
                }

                if (!verificationResult.IsAudienceValid)
                {
                    Console.WriteLine($"Reason: JWT Token is not for this Audience. Required Audience: {req_aud}, Received Audience: {verificationResult.ReceivedAudience}");
                }

                if (!verificationResult.IsSignatureValid)
                {
                    Console.WriteLine("Reason: JWT Token Signature is not Verified");
                }
            }
        }
    }

    public class JwtTokenVerifier
    {
        private readonly JwtSecurityToken jwt;
        private readonly string requiredAudience;
        private readonly string keyEndpoint;

        public JwtTokenVerifier(string jwtSource, string requiredAudience, string endpoint = "https://keystore.openbankingtest.org.uk/0014H00003ARnZ4QAL/0014H00003ARnZ4QAL.jwks")
        {
            jwt = GetJwtToken(jwtSource);
            this.requiredAudience = requiredAudience;
            keyEndpoint = endpoint;
        }

        public JwtVerificationResult Verify()
        {
            var result = new JwtVerificationResult
            {
                IsExpirationValid = VerifyExpiration(),
                IsAudienceValid = VerifyAudience(),
                IsSignatureValid = VerifySignature(),
                ReceivedAudience = jwt.Audiences.First()
            };

            return result;
        }

        private static JwtSecurityToken GetJwtToken(string source, bool isRaw = true)
        {
            string jwt = isRaw ? source : File.ReadAllText(source);
            return new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        }

        private string GetPublicKey(bool raw = true)
        {
            var json = new System.Net.WebClient().DownloadString(keyEndpoint);
            var jobj = System.Text.Json.JsonDocument.Parse(json);
            var keys = jobj.RootElement.GetProperty("keys");

            foreach (var key in keys.EnumerateArray())
            {
                var extractedKid = key.GetProperty("kid").GetString();

                if (extractedKid == jwt.Header.Kid)
                {
                    var x5c = key.GetProperty("x5c").EnumerateArray().First().GetString();

                    if (string.IsNullOrEmpty(x5c))
                    {
                        return null;
                    }
                    return raw ? x5c : $"-----BEGIN CERTIFICATE-----\n{x5c}\n-----END CERTIFICATE-----";
                }
            }
            return null;
        }

        private bool VerifyExpiration()
        {
            var now = DateTime.UtcNow;
            var iss = jwt.ValidFrom;
            var exp = jwt.ValidTo;
            return now >= iss && now <= exp;
        }

        private bool VerifyAudience()
        {
            return jwt.Audiences.Contains(requiredAudience);
        }

        private bool VerifySignature()
        {
            var formatted = Encoding.UTF8.GetBytes($"{jwt.RawHeader}.{jwt.RawPayload}");
            var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
            var publicKey = GetPublicKey();

            if (publicKey == null)
            {
                return false;
            }

            var sigVerify = new X509Certificate2(Convert.FromBase64String(publicKey))
                .GetRSAPublicKey()!
                .VerifyData(
                    data: formatted,
                    signature: signature,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    padding: RSASignaturePadding.Pss
                );

            return sigVerify;
        }
    }

    public class JwtVerificationResult
    {
        public bool IsSuccess => IsExpirationValid && IsAudienceValid && IsSignatureValid;
        public required bool IsExpirationValid { get; set; }
        public required bool IsAudienceValid { get; set; }
        public required bool IsSignatureValid { get; set; }
        public required string ReceivedAudience { get; set; }
    }
}
