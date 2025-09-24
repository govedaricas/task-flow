using System.Security.Cryptography;
using Application.Interfaces;

namespace Persistance.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100000, HashAlgorithmName.SHA256);
            var salt = deriveBytes.Salt;
            var key = deriveBytes.GetBytes(32);

            var result = new byte[salt.Length + key.Length];

            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(key, 0, result, salt.Length, key.Length);

            return Convert.ToBase64String(result);
        }

        public bool Verify(string password, byte[] storedHash)
        {
            var salt = new byte[16];
            Buffer.BlockCopy(storedHash, 0, salt, 0, 16);

            var key = new byte[32];
            Buffer.BlockCopy(storedHash, 16, key, 0, 32);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var newKey = deriveBytes.GetBytes(32);

            return key.SequenceEqual(newKey);
        }
    }
}
