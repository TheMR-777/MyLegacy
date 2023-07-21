
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
		static readonly string jwt_raw = "eyJhbGciOiJQUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6ImFwcGxpY2F0aW9uL2pzb24iLCJraWQiOiJwVk1vbVExTndCNm5LdVNBeGFRRE0wUHJCZzAifQ.eyJleHAiOjE2OTI1NDMyNDEsImlzcyI6ImczSGw3YVVxT0djenJteHlyWDRvWUwiLCJhdWQiOiIwMDE0SDAwMDAzQVJuYTNRQUQiLCJpYXQiOjE2ODk4NjQ4NDEsImp0aSI6ImRkZWVkNjk5LTExMWMtNDk0NC05NTE1LTdjNmU5NTc4YThjNSIsInRva2VuX2VuZHBvaW50X2F1dGhfbWV0aG9kIjoicHJpdmF0ZV9rZXlfand0Iiwic29mdHdhcmVfaWQiOiJnM0hsN2FVcU9HY3pybXh5clg0b1lMIiwiY2xpZW50X2lkIjoiZzNIbDdhVXFPR2N6cm14eXJYNG9ZTCIsImdyYW50X3R5cGVzIjpbImNsaWVudF9jcmVkZW50aWFscyJdLCJyZWRpcmVjdF91cmlzIjpbImh0dHBzOi8vYWNldW5pb24uY29tLyJdLCJyZXNwb25zZV90eXBlcyI6WyJjb2RlIGlkX3Rva2VuIl0sInNjb3BlIjoib3BlbmlkIG5hbWUtdmVyaWZpY2F0aW9uIiwiYXBwbGljYXRpb25fdHlwZSI6IndlYiIsInNvZnR3YXJlX3N0YXRlbWVudCI6ImV5SmhiR2NpT2lKUVV6STFOaUlzSW10cFpDSTZJbVI2Y1hWM1UxUnViVUZpTjBvd01XUldSR1pKZDJveFMxY3RZVVE0TTFSWVRURnRWbUp2T1d0a1JXczlJaXdpZEhsd0lqb2lTbGRVSW4wLmV5SnBjM01pT2lKUGNHVnVRbUZ1YTJsdVp5Qk1kR1FpTENKcFlYUWlPakUyT0RNM01UQXhNRGtzSW1wMGFTSTZJamcxT1dJMVpEUTVPRGt4WkRRNU9Ua2lMQ0p6YjJaMGQyRnlaVjlsYm5acGNtOXViV1Z1ZENJNkluTmhibVJpYjNnaUxDSnpiMlowZDJGeVpWOXRiMlJsSWpvaVZHVnpkQ0lzSW5OdlpuUjNZWEpsWDJsa0lqb2laek5JYkRkaFZYRlBSMk42Y20xNGVYSllORzlaVENJc0luTnZablIzWVhKbFgyTnNhV1Z1ZEY5cFpDSTZJbWN6U0d3M1lWVnhUMGRqZW5KdGVIbHlXRFJ2V1V3aUxDSnpiMlowZDJGeVpWOWpiR2xsYm5SZmJtRnRaU0k2SWtGRFJTQlZUa2xQVGlCTVNVMUpWRVZFSWl3aWMyOW1kSGRoY21WZlkyeHBaVzUwWDJSbGMyTnlhWEIwYVc5dUlqb2lRVU5GSUZWdWFXOXVJRXhwYldsMFpXUWdLT0tBbkVGRFJTQlZibWx2YnVLQW5Ta2dhWE1nWVNCamIyMXdZVzU1SUdsdVkyOXljRzl5WVhSbFpDQjFibVJsY2lCMGFHVWdiR0YzY3lCdlppQkZibWRzWVc1a0lHRnVaQ0JYWVd4bGN5QjNhWFJvSUdOdmJYQmhibmtnY21WbmFYTjBjbUYwYVc5dUlHNTFiV0psY2lBd09UVXpNREl6TXk0Z1ZHaGxJSEpsWjJsemRHVnlaV1FnYjJabWFXTmxJR0ZrWkhKbGMzTWdhWE1nVUdsalkyRmthV3hzZVNCSWIzVnpaU3dnTkRrZ1VHbGpZMkZrYVd4c2VTd2dUV0Z1WTJobGMzUmxjaUJOTVNBeVFWQXNJRlZ1YVhSbFpDQkxhVzVuWkc5dExpSXNJbk52Wm5SM1lYSmxYM1psY25OcGIyNGlPaUl4TGpBaUxDSnpiMlowZDJGeVpWOWpiR2xsYm5SZmRYSnBJam9pYUhSMGNITTZMeTloWTJWMWJtbHZiaTVqYjIwdklpd2ljMjltZEhkaGNtVmZjbVZrYVhKbFkzUmZkWEpwY3lJNld5Sm9kSFJ3Y3pvdkwyRmpaWFZ1YVc5dUxtTnZiUzhpWFN3aWMyOW1kSGRoY21WZmNtOXNaWE1pT2xzaVEwOVFVbVZ4ZFdWemRHVnlJaXdpUTA5UVVtVnpjRzl1WkdWeUlsMHNJbTl5WjJGdWFYTmhkR2x2Ymw5amIyMXdaWFJsYm5SZllYVjBhRzl5YVhSNVgyTnNZV2x0Y3lJNmV5SmhkWFJvYjNKcGRIbGZhV1FpT2lKUVFWbFZTMGRDVWlJc0luSmxaMmx6ZEhKaGRHbHZibDlwWkNJNklqRXdNREE0TWlJc0luTjBZWFIxY3lJNklrRmpkR2wyWlNJc0ltRjFkR2h2Y21sellYUnBiMjV6SWpwYmV5SnRaVzFpWlhKZmMzUmhkR1VpT2lKSFFpSXNJbkp2YkdWeklqcGJJa05QVUZKbGNYVmxjM1JsY2lJc0lrTlBVRkpsYzNCdmJtUmxjaUpkZlYxOUxDSnpiMlowZDJGeVpWOXNiMmR2WDNWeWFTSTZJbWgwZEhCek9pOHZZV05sZFc1cGIyNHVZMjl0THlJc0ltOXlaMTl6ZEdGMGRYTWlPaUpCWTNScGRtVWlMQ0p2Y21kZmFXUWlPaUl3TURFMFNEQXdNREF6UVZKdVdqUlJRVXdpTENKdmNtZGZibUZ0WlNJNklrRkRSU0JWVGtsUFRpQk1TVTFKVkVWRUlpd2liM0puWDJOdmJuUmhZM1J6SWpwYmV5SnVZVzFsSWpvaVZHVmphRzVwWTJGc0lpd2laVzFoYVd3aU9pSjBaV05vTG5OMWNIQnZjblJBWVdObGRXNXBiMjR1WTI5dElpd2ljR2h2Ym1VaU9pSk5kV2hoYlcxaFpDQlpiM1Z6WVdZZ1FXeHBJaXdpZEhsd1pTSTZJbFJsWTJodWFXTmhiQ0o5WFN3aWIzSm5YMnAzYTNOZlpXNWtjRzlwYm5RaU9pSm9kSFJ3Y3pvdkwydGxlWE4wYjNKbExtOXdaVzVpWVc1cmFXNW5kR1Z6ZEM1dmNtY3VkV3N2TURBeE5FZ3dNREF3TTBGU2JsbzBVVUZNTHpBd01UUklNREF3TUROQlVtNWFORkZCVEM1cWQydHpJaXdpYjNKblgycDNhM05mY21WMmIydGxaRjlsYm1Sd2IybHVkQ0k2SW1oMGRIQnpPaTh2YTJWNWMzUnZjbVV1YjNCbGJtSmhibXRwYm1kMFpYTjBMbTl5Wnk1MWF5OHdNREUwU0RBd01EQXpRVkp1V2pSUlFVd3ZjbVYyYjJ0bFpDOHdNREUwU0RBd01EQXpRVkp1V2pSUlFVd3VhbmRyY3lJc0luTnZablIzWVhKbFgycDNhM05mWlc1a2NHOXBiblFpT2lKb2RIUndjem92TDJ0bGVYTjBiM0psTG05d1pXNWlZVzVyYVc1bmRHVnpkQzV2Y21jdWRXc3ZNREF4TkVnd01EQXdNMEZTYmxvMFVVRk1MMmN6U0d3M1lWVnhUMGRqZW5KdGVIbHlXRFJ2V1V3dWFuZHJjeUlzSW5OdlpuUjNZWEpsWDJwM2EzTmZjbVYyYjJ0bFpGOWxibVJ3YjJsdWRDSTZJbWgwZEhCek9pOHZhMlY1YzNSdmNtVXViM0JsYm1KaGJtdHBibWQwWlhOMExtOXlaeTUxYXk4d01ERTBTREF3TURBelFWSnVXalJSUVV3dmNtVjJiMnRsWkM5bk0waHNOMkZWY1U5SFkzcHliWGg1Y2xnMGIxbE1MbXAzYTNNaUxDSnpiMlowZDJGeVpWOXdiMnhwWTNsZmRYSnBJam9pYUhSMGNITTZMeTloWTJWMWJtbHZiaTVqYjIwdklpd2ljMjltZEhkaGNtVmZkRzl6WDNWeWFTSTZJbWgwZEhCek9pOHZZV05sZFc1cGIyNHVZMjl0THlJc0luTnZablIzWVhKbFgyOXVYMkpsYUdGc1psOXZabDl2Y21jaU9tNTFiR3g5Lk9kMTA5c3pEaXBYZXh2V1lkbEszSTZaTWN5aHN1UDBQUnpndmI2aU5Id09WbkVGTGRnSnA3X0wwbUNYXy00Q3B1VTJxQ1pZQlVIb3RnZmFsUU5jZzh5UVQ0enFmOF9BTXA2ekJ0Z19aci04NXVzakZKVDJ1LWhJY01hSDJRMHNVOGJ6SVhhLWdJc0JIa1dvWlkzQkRLSWRqNGk3SFZnQzFhejlFZUFDOGdVd1ZPWE1yV0RoUy1fUy1VT2pkX1RCZ256RG9NT0twTV96QW1IQTFWZ20zV3JmcFNSQ2ZqNmVpSjBuc1padndubWJSTWVNSDJJd25FSlBKVU1hdzNydk1QaTF1NUZjWFExZ3owSDRHYUVwLUU5NHpqMUlDMFIxbjQyek1GcThHMlJtc3pydjR5UmJXbVdnOWxPRFRPbktZcmF3cDZsQnlMbGFFZENLX1RkWURZdyIsImlkX3Rva2VuX3NpZ25lZF9yZXNwb25zZV9hbGciOiJQUzI1NiIsInJlcXVlc3Rfb2JqZWN0X3NpZ25pbmdfYWxnIjoiUFMyNTYiLCJ0b2tlbl9lbmRwb2ludF9hdXRoX3NpZ25pbmdfYWxnIjoiUFMyNTYifQ.ZFadoOIBHYmZW8ve-lhpnmrStXL6FD0VUec9OLa_EdtLTKuxh27YLmKbgJ0CyUGo_LF4URs7rSoGdELxBJ15uu4qCmWxifNhcKbXvsucIxnKUJVxXGOiAleu1QyndeXrAo-njifBLtSdXoz4EE6Z9eD7DmKjLcjaISrlYL6IWIE7KpJNRnTlzRPzLReM15Zb_ujjZ00U0Vc_wKapKgLwFuzYZlekA2_6hlhJQu8R2-Fq6rGcudbb_aJ0gKsU1VQ62Y2NgIsJ0Mp39Z6WeEXA0HvC4ymMpGx0fYi3EwE4VLKf4DVyRzxrv8yAjOKIPq-tZNiln49ejTpCtaAcwk6Zuw";

		public static void Main()
		{
			var jwt = Get.JWT(source: jwt_raw);
			var key = Get.GetPublicKeyFrom(jwt.Header.Kid);

			// Verify Expiration
			var now = DateTime.UtcNow;
			var iss = jwt.ValidFrom;
			var exp = jwt.ValidTo;
			var exp_verified = now >= iss && now <= exp;

			// Verify Audience
			var aud = jwt.Audiences.First();
			var aud_verified = aud == "0014H00003ARna3QAD";
			
			// Verify Signature
			var formatted = Encoding.UTF8.GetBytes($"{jwt.RawHeader}.{jwt.RawPayload}");
			var signature = Base64UrlEncoder.DecodeBytes(jwt.RawSignature);
			var verified = new X509Certificate2(Convert.FromBase64String(key))
				.GetRSAPublicKey()!
				.VerifyData(
					data: formatted,
					signature: signature,
					hashAlgorithm: HashAlgorithmName.SHA256,
					padding: RSASignaturePadding.Pss
			);

			Console.WriteLine($"Signature  Verification : {verified}");
			Console.WriteLine($"Audience   Verification : {aud_verified}");
			Console.WriteLine($"Expiration Verification : {exp_verified}");
			Console.WriteLine('\n');

			// Decoded JWT
			var option = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
			var header = System.Text.Json.JsonSerializer.Serialize(jwt.Header, option);
			var payload = System.Text.Json.JsonSerializer.Serialize(jwt.Payload, option);
			var sign = jwt.RawSignature;

			Console.WriteLine($"Header   : {header}");
			Console.WriteLine($"Payload  : {payload}");
			Console.WriteLine($"Signature: {sign}");
		}
	}

	public class Get
	{
		public static JwtSecurityToken JWT(string source, bool isRaw = true)
		{
			string jwt = isRaw ? source : File.ReadAllText(source);
			return new JwtSecurityTokenHandler().ReadJwtToken(jwt);
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
