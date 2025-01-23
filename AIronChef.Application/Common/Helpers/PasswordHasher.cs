using AIronChef.Domain.Models;
using Konscious.Security.Cryptography;
using System.Text;

namespace AIronChef.Application.Common.Helpers
{
    internal class PasswordHasher
    {
        public static string HashPassword(string password, string salt)
        {
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = Convert.FromBase64String(salt);
                hasher.DegreeOfParallelism = 8; // Number of threads
                hasher.MemorySize = 65536; // 64 MB of memory
                hasher.Iterations = 4; // Number of iterations
                var hash = hasher.GetBytes(64); // Get 64-byte hash
                return Convert.ToBase64String(hash); // Convert to string
            }
        }

        public static string GenerateSalt()
        {
            int length = 16;
            var salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
            return Convert.ToBase64String(salt); // Convert to string
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var passwordHash = HashPassword(password, storedSalt);
            return passwordHash == storedHash;
        }

        public static bool VerifyPassword(string password, User user)
        {
            return VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }
    }
}
