using System.Security.Cryptography;
using System.Text;

namespace MyPlayground
{
    public class AesGcmEncryption
    {
        /// <summary>
        /// Encrypts the given string using the final key (as described in the official documentation).
        /// </summary>
        /// <remarks>
        /// Specifications: <br></br>
        /// key: 256-bit / 32 bytes, Base64 encoded <br></br>
        /// nonce: 96-bit / 12 bytes, Base64 encoded <br></br>
        /// Expected finalKey format: "key|nonce"
        /// </remarks>
        /// <param name="source">Source string to encrypt</param>
        /// <param name="finalKey">Final key to use for encryption (key|nonce)</param>
        /// <returns>
        /// Pair of strings: (auth-code, cipher) <br></br>
        /// Both strings are Base64 encoded
        /// </returns>
        public static (string, string) Encrypt(string source, string finalKey)
        {
            var keyAndNonce = finalKey.Split('|');
            var key = Convert.FromBase64String(keyAndNonce.First());
            var nonce = Convert.FromBase64String(keyAndNonce.Last());

            var rawSource = Encoding.UTF8.GetBytes(source);
            var rawBuffer = new byte[rawSource.Length];
            var authCodes = new byte[16];

            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(
                nonce: nonce,
                plaintext: rawSource,
                ciphertext: rawBuffer,
                tag: authCodes
            );

            return (Convert.ToBase64String(authCodes), Convert.ToBase64String(rawBuffer));
        }

        /// <summary>
        /// Decrypts the given string using the final key, and auth-code (as described in the official documentation).
        /// </summary>
        /// <param name="cipher">Encrypted string</param>
        /// <param name="finalKey">Final key to use for decryption (key|nonce)</param>
        /// <param name="tag">Authentication Code (tag) to use for decryption</param>
        /// <remarks>
        /// Specifications: <br></br>
        /// key: 256-bit / 32 bytes, Base64 encoded <br></br>
        /// tag: 128-bit / 16 bytes, Base64 encoded <br></br>
        /// nonce: 96-bit / 12 bytes, Base64 encoded <br></br>
        /// Expected finalKey format: "key|nonce" <br></br>
        /// </remarks>
        /// <returns>Decrypted string (UTF-8)</returns>
        public static string Decrypt(string cipher, string finalKey, string tag)
        {
            var keyAndNonce = finalKey.Split('|');
            var key = Convert.FromBase64String(keyAndNonce.First());
            var nonce = Convert.FromBase64String(keyAndNonce.Last());

            var rawCipher = Convert.FromBase64String(cipher);
            var authCodes = Convert.FromBase64String(tag);
            var rawBuffer = new byte[rawCipher.Length];

            using var aesGcm = new AesGcm(key);
            aesGcm.Decrypt(
                nonce: nonce,
                ciphertext: rawCipher,
                plaintext: rawBuffer,
                tag: authCodes
            );

            return Encoding.UTF8.GetString(rawBuffer);
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
                Print($"Selected Final-Key    : {finalKey}\n");

                Console.Write("Enter text to encrypt : ");
                var rawData = Console.ReadLine()!;

                var (tag, encryptedData) = AesGcmEncryption.Encrypt(rawData, finalKey);
                Print("");
                Print($"Encrypted Text        : {encryptedData}");
                Print($"Authentication Code   : {tag}\n");

                var decryptedData = AesGcmEncryption.Decrypt(encryptedData, finalKey, tag);
                Print($"Decrypted Text        : {decryptedData}\n");

                Print("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
