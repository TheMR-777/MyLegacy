using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MyPlayground
{
    public static class AES
    {
        /// <summary>
        /// Encrypts or Decrypts the given string using the final-key <br></br> 
        /// (as described in the official documentation).
        /// </summary>
        /// <remarks>
        /// Specifications: <br></br>
        /// key: 256-bit / 32 bytes, Base64 encoded <br></br>
        /// nonce: 96-bit / 12 bytes, Base64 encoded <br></br>
        /// Expected finalKey format: "key|nonce"
        /// </remarks>
        /// <param name="source">Source string to encrypt/decrypt </param>
        /// <param name="finalKey">Final key to use for encryption/decryption (key|nonce) </param>
        /// <param name="toEncrypt"><c>true</c> to encrypt, <c>false</c> to decrypt </param>
        /// <returns> Encrypted/Decrypted string (Base64 encoded) </returns>
        public static string? GCM(string source, string finalKey, bool toEncrypt = true, int macSize = 128)
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
                    ? Encoding.UTF8.GetBytes(source)
                    : Convert.FromBase64String(source);
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
                    ? Convert.ToBase64String(rawBuffer)
                    : Encoding.UTF8.GetString(rawBuffer);
            }
            catch
            {
                return null;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var Print = new Action<string>(Console.WriteLine);

            while (true)
            {
                Print("AES-GCM Encryption and Decryption");
                Print("---------------------------------");

                var key = RandomNumberGenerator.GetBytes(32);    // 256-bit key
                var nonce = RandomNumberGenerator.GetBytes(12);  // 96-bit nonce

                var finalKey = $"{Convert.ToBase64String(key)}|{Convert.ToBase64String(nonce)}";
                Print($"Selected Final-Key    : {finalKey}");

                Console.Write("Enter text to encrypt : ");
                var sourceText = Console.ReadLine()!;

                var encryptedData = AES.GCM(sourceText, finalKey)!;
                Print("");
                Print($"Encrypted Text        : {encryptedData}");

                var decryptedData = AES.GCM(encryptedData, finalKey, toEncrypt: false);
                Print($"Decrypted Text        : {decryptedData}\n");

                Print("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
