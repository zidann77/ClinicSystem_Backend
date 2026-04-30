using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLayer
{
   public static class clsPasswordHasher
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                10000,
                HashAlgorithmName.SHA256
            );

            byte[] hash = pbkdf2.GetBytes(32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] stored = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                10000,
                HashAlgorithmName.SHA256
            );

            byte[] computed = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(stored, computed);
        }
    }
}
