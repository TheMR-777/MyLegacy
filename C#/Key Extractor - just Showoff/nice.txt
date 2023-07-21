
static async Task<string> PublicKeyFromAsync(string kid)
{
    try
    {
        var kloc = "https://keystore.openbankingtest.org.uk/0014H00003ARnZ4QAL/0014H00003ARnZ4QAL.jwks";
        var json = await new HttpClient().GetStringAsync(kloc);
        var jobj = System.Text.Json.JsonDocument.Parse(json);
        var keys = jobj.RootElement.GetProperty("keys");

        foreach (var key in keys.EnumerateArray())
        {
            var extracted_kid = key.GetProperty("kid").GetString();

            if (extracted_kid == kid)
            {
                var x5c = key.GetProperty("x5c")[0];
                return $"-----BEGIN CERTIFICATE-----\n{x5c.GetString()}\n-----END CERTIFICATE-----";
            }
        }
        return "";
    }
    catch
    {
        return "";
    }
}

var kid = "pVMomQ1NwB6nKuSAxaQDM0PrBg0";
var key = await PublicKeyFromAsync(kid);
Console.WriteLine(key);
