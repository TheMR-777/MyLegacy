using System.Security.Cryptography;

namespace MySpace
{
    /* -------------------------------------
     * AES-256-ECB Encryption and Decryption
     * ------------------------------------- */
    class MyProgram
    {
        private const string Key = "06+bPn7ht6TB3o92Oz/avPc5zL/KH29tBhPJNOUGZs8=";

        static void Main()
        {
            var my_data = "So it worked!";

            var encrypt = Encrypt(System.Text.Encoding.UTF8.GetBytes(my_data));
            var decrypt = Decrypt(encrypt);

            var result = System.Text.Encoding.UTF8.GetString(decrypt);
        }

        public static byte[] Encrypt(byte[] input)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(Key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(input, 0, input.Length);
        }

        public static byte[] Decrypt(byte[] input)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(Key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(input, 0, input.Length);
        }
    }
}
