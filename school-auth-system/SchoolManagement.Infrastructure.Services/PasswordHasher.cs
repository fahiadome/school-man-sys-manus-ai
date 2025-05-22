using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SchoolManagement.Core.Domain.Services;

namespace SchoolManagement.Infrastructure.Services
{
    /// <summary>
    /// Password hasher implementation using PBKDF2
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8; // 128 bits
        private const int KeySize = 256 / 8; // 256 bits
        private const int Iterations = 10000;
        private static readonly KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

        /// <summary>
        /// Hashes a password
        /// </summary>
        public (string passwordHash, string salt) HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // Generate a random salt
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Hash the password with the salt
            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: Prf,
                iterationCount: Iterations,
                numBytesRequested: KeySize);

            // Convert to base64 strings
            string saltString = Convert.ToBase64String(saltBytes);
            string hashString = Convert.ToBase64String(hashBytes);

            return (hashString, saltString);
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash, string salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentNullException(nameof(passwordHash));
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));

            // Convert salt from base64 string
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the input password with the same salt
            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: Prf,
                iterationCount: Iterations,
                numBytesRequested: KeySize);

            // Convert to base64 string
            string hashString = Convert.ToBase64String(hashBytes);

            // Compare the hashes
            return hashString == passwordHash;
        }
    }
}
