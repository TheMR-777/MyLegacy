using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

class JWTGenerator
{
    static void Main()
    {
        string kid = "pVMomQ1NwB6nKuSAxaQDM0PrBg0";
        string key_PEM = @"D:\PayUK\obsigning.key";

        var payload = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "Data", new Dictionary<string, string>
                {
                    { "SchemeName", "SortCodeAccountNumber" },
                    { "AccountType", "Personal" },
                    { "Identification", "30241433264517" },
                    { "Name", ",,Mr Lance Henry" }
                }
            }
        };
        var headers = new Dictionary<string, object>
        {
            { "alg", "PS256" },
            { "typ", "JOSE" },
            { "cty", "application/json" },
            { "kid", kid },
            { "http://openbanking.org.uk/iat", Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds) },
            { "http://openbanking.org.uk/iss", "0014H00003ARnZ4QAL/g3Hl7aUqOGczrmxyrX4oYL" },
            { "http://openbanking.org.uk/tan", "openbankingtest.org.uk" },
            { "crit", new string[] { "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss", "http://openbanking.org.uk/tan" } }
        };
        var JWToken = GenerateJWT(payload, headers, key_PEM);
        
        var tokenParts = JWToken.Split('.');
        Console.WriteLine(tokenParts[0] + ".." + tokenParts[2]);
    }

    /// <summary>
    /// Generates a JWT token
    /// </summary>
    /// <remarks>
    /// The payload and headers are passed as dictionaries, as the payload can be nested
    /// </remarks>
    /// <param name="payload"> The payload (body) of the JWT token </param>
    /// <param name="headers"> The headers of the JWT token </param>
    /// <param name="keyPath"> The path to the private key (of PEM format) </param>
    /// <returns> The generated JWT token </returns>
    static string GenerateJWT(Dictionary<string, Dictionary<string, string>> payload, Dictionary<string, object> headers, string keyPath)
    {
        var privateKey = GetPrivateKey(keyPath);
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateJwtSecurityToken(
            issuer: headers["http://openbanking.org.uk/iss"].ToString(),
            audience: headers["http://openbanking.org.uk/tan"].ToString(),
            subject: null,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            issuedAt: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(privateKey, SecurityAlgorithms.RsaSsaPssSha256)
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

    /// <summary>
    /// Gets the private key from the given path
    /// </summary>
    /// <remarks>
    /// The private key is expected to be in PEM format
    /// </remarks>
    /// <param name="keyPath"> The path to the private key (of PEM format) </param>
    /// <returns> The private key </returns>
    static RsaSecurityKey GetPrivateKey(string keyPath)
    {
        var privateKeyText = File.ReadAllText(keyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyText);
        return new RsaSecurityKey(privateKey);
    }
}
