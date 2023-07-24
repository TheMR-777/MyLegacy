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
            try
            {
                new JwtTokenVerifier(
                    jwtSource: File.ReadAllText(jwt_src), 
                    requiredAudience: req_aud
                ).Verify();

                Console.WriteLine("JWT Token Verified Successfully");
            }
            catch (JwtTokenVerificationException ex)
            {
                Console.WriteLine($"----- Failed to Verify JWT Token -----\nReason : {ex.Message}");
            }
        }
    }

    public class JwtTokenVerifier
    {
        private readonly JwtSecurityToken jwt;
        private readonly string required_aud;
        private readonly string key_endpoint;

        public JwtTokenVerifier(string jwtSource, string requiredAudience, string endpoint = "https://keystore.openbankingtest.org.uk/0014H00003ARnZ4QAL/0014H00003ARnZ4QAL.jwks")
        {
            jwt = GetJwtToken(jwtSource);
            required_aud = requiredAudience;
            key_endpoint = endpoint;
        }

        public bool Verify()
        {
            return new bool[]
            {
                VerifyExpiration(),
                VerifyAudience(),
                VerifySignature()
            }.All(_ => _);
        }

        private static JwtSecurityToken GetJwtToken(string source, bool isRaw = true)
        {
            string jwt = isRaw ? source : File.ReadAllText(source);
            return new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        }

        private string GetPublicKey(bool raw = true)
        {
            var json = new System.Net.WebClient().DownloadString(key_endpoint);
            var jobj = System.Text.Json.JsonDocument.Parse(json);
            var keys = jobj.RootElement.GetProperty("keys");

            foreach (var key in keys.EnumerateArray())
            {
                var extracted_kid = key.GetProperty("kid").GetString();

                if (extracted_kid == jwt.Header.Kid)
                {
                    var x5c = key.GetProperty("x5c").EnumerateArray().First().GetString();

                    if (string.IsNullOrEmpty(x5c))
                    {
                        throw new JwtTokenVerificationException("x5c is empty");
                    }
                    return raw ? x5c : $"-----BEGIN CERTIFICATE-----\n{x5c}\n-----END CERTIFICATE-----";
                }
            }
            throw new JwtTokenVerificationException("kid not found");
        }

        private bool VerifyExpiration()
        {
            var now = DateTime.UtcNow;
            var iss = jwt.ValidFrom;
            var exp = jwt.ValidTo;
            if (!(now >= iss && now <= exp))
            {
                throw new JwtTokenVerificationException("JWT Token is Expired");
            }
            return true;
        }

        private bool VerifyAudience()
        {
            foreach (var aud in jwt.Audiences)
            {
                if (aud == required_aud)
                {
                    return true;
                }
            }
            throw new JwtTokenVerificationException($"JWT Token is not for this Audience. Required Audience: {required_aud}, Received Audience: {jwt.Audiences.First()}");
        }

        private bool VerifySignature()
        {
            var formatted = Encoding.UTF8.GetBytes($"{jwt.RawHeader}.{jwt.RawPayload}");
            var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
            var sigVerify = new X509Certificate2(Convert.FromBase64String(GetPublicKey()))
                .GetRSAPublicKey()!
                .VerifyData(
                    data: formatted,
                    signature: signature,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    padding: RSASignaturePadding.Pss
            );

            if (!sigVerify)
            {
                throw new JwtTokenVerificationException("JWT Token Signature is not Verified");
            }
            return true;
        }
    }

    public class JwtTokenVerificationException : Exception
    {
        public JwtTokenVerificationException(string message) : base(message) { }
    }
}
