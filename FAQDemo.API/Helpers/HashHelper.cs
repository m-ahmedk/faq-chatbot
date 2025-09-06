using System.Security.Cryptography;
using System.Text;

namespace FAQDemo.API.Helpers
{
    public static class HashHelper
    {
        public static (byte[] hash, byte[] salt) HashPassword(string password)
        {
            // extra validation, as can be accessed directly for e.g. unit testing
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            using var hmac = new HMACSHA256();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, hmac.Key); // Key = PasswordStamp
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            // extra validation, as can be accessed directly for e.g. unit testing
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            using var hmac = new HMACSHA256(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}