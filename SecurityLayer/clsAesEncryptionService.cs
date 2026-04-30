using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLayer
{
    public static class clsAesEncryptionService
    {
        public static string Encrypt(string plainText, string key)
        {
            using var aes = Aes.Create();

            aes.Key = DeriveKey(key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

            return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(encrypted)}";
        }

        public static string Decrypt(string cipherText, string key)
        {
            var parts = cipherText.Split(':');

            byte[] iv = Convert.FromBase64String(parts[0]);
            byte[] data = Convert.FromBase64String(parts[1]);

            using var aes = Aes.Create();

            aes.Key = DeriveKey(key);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static byte[] DeriveKey(string key)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }
}
