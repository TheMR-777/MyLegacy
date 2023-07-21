
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Project
{
    class Program
    {
        public static void Main()
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Utility.GetJWT());
            var crt = new X509Certificate2(
                rawData: Convert.FromBase64String(Utility.GetPublicKeyFrom(jwt.Header.Kid))
            );

            if (crt == null)
            {
                Console.WriteLine("Certificate not found");
                return;
            }

            var formatted = Encoding.UTF8.GetBytes($"{jwt.RawHeader}.{jwt.RawPayload}");
            var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
            var verified = crt.GetRSAPublicKey()!.VerifyData(
                data: formatted,
                signature: signature,
                hashAlgorithm: HashAlgorithmName.SHA256,
                padding: RSASignaturePadding.Pss
            );

            Console.WriteLine($"Signature Verification: {verified}");
        }
    }

    public class Utility
    {
        public static string GetJWT(string filePath = @"D:\PayUK\jxt.txt")
        {
            var jwt = File.ReadAllText(filePath);
            return jwt;
        }

        public static string GetPublicKeyFrom(string kid = "pVMomQ1NwB6nKuSAxaQDM0PrBg0", string endpoint = "https://keystore.openbankingtest.org.uk/0014H00003ARnZ4QAL/0014H00003ARnZ4QAL.jwks", bool raw = true)
        {
            try
            {
                var json = new System.Net.WebClient().DownloadString(endpoint);
                var jobj = System.Text.Json.JsonDocument.Parse(json);
                var keys = jobj.RootElement.GetProperty("keys");

                foreach (var key in keys.EnumerateArray())
                {
                    var extracted_kid = key.GetProperty("kid").GetString();

                    if (extracted_kid == kid)
                    {
                        var x5c = key.GetProperty("x5c").EnumerateArray().First().GetString();

                        if (string.IsNullOrEmpty(x5c))
                        {
                            Console.WriteLine("x5c is empty");
                            return string.Empty;
                        }
                        if (!raw)
                        {
                            return $"-----BEGIN CERTIFICATE-----\n{x5c}\n-----END CERTIFICATE-----";
                        }
                        return x5c;
                    }
                }
                Console.WriteLine("kid not found");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
