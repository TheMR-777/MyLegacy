using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

class JWTGenerator
{
	static void Main()
	{
        var JWToken = GenerateJWT(
			accountType: "Personal",
			identification: "30241433264517",
			name: ",,Mr Lance Henry"
		);
		
		if (!string.IsNullOrEmpty(JWToken))
		{
            var tokenParts = JWToken.Split('.');
            Console.WriteLine(tokenParts[0] + ".." + tokenParts[2]);
        }
		else
		{
			Console.WriteLine(JWToken);
		}
	}

	/// <summary>
	/// Generates a JWT token
	/// </summary>
	/// <remarks>
	/// The JWT token is generated using the private key from the obsigning.key file
	/// </remarks>
	/// <param name="accountType"> The account type </param>
	/// <param name="identification"> The account identification </param>
	/// <param name="name"> The name of account holder </param>
	/// <param name="kid"> The key-id </param>
	/// <returns> The generated JWT token </returns>
	static string GenerateJWT(string accountType, string identification, string name, string kid = "pVMomQ1NwB6nKuSAxaQDM0PrBg0")
	{
		try
		{
			var keyPath = @"D:\PayUK\obsigning.key";
			var key_PEM = RSA.Create(); key_PEM.ImportFromPem(File.ReadAllText(keyPath));
			var headers = new Dictionary<string, object>
		{
			{ "alg", "PS256" },
			{ "typ", "JOSE" },
			{ "cty", "application/json" },
			{ "kid", kid },
			{ "http://openbanking.org.uk/iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
			{ "http://openbanking.org.uk/iss", "0014H00003ARnZ4QAL/g3Hl7aUqOGczrmxyrX4oYL" },
			{ "http://openbanking.org.uk/tan", "openbankingtest.org.uk" },
			{ "crit", new string[] { "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss", "http://openbanking.org.uk/tan" } }
		};
			var payload = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"Data", new Dictionary<string, string>
				{
					{ "SchemeName", "SortCodeAccountNumber" },
					{ "AccountType", accountType },
					{ "Identification", identification },
					{ "Name", name }
				}
			}
		};

			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.CreateJwtSecurityToken(
				issuer: headers["http://openbanking.org.uk/iss"].ToString(),
				audience: headers["http://openbanking.org.uk/tan"].ToString(),
				subject: null,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(10),
				issuedAt: DateTime.UtcNow,
				signingCredentials: new SigningCredentials(
					key: new RsaSecurityKey(key_PEM),
					algorithm: SecurityAlgorithms.RsaSsaPssSha256
				)
			);

			tokenHandler.OutboundClaimTypeMap.Clear();
			securityToken.Payload.Clear();
			foreach (var (key, value) in payload)
				securityToken.Payload.Add(key, value);

			securityToken.Header.Clear();
			foreach (var (key, value) in headers)
				securityToken.Header.Add(key, value);

			return tokenHandler.WriteToken(securityToken);
		}
        catch
		{
			return "";
        }
	}
}
