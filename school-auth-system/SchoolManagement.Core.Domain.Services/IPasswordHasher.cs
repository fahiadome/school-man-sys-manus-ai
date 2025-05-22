using System;

namespace SchoolManagement.Core.Domain.Services
{
    /// <summary>
    /// Interface for password hashing and verification
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>The hash and salt</returns>
        (string hash, string salt) HashPassword(string password);
        
        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="hash">The hash to verify against</param>
        /// <param name="salt">The salt used to generate the hash</param>
        /// <returns>True if the password is valid, false otherwise</returns>
        bool VerifyPassword(string password, string hash, string salt);
    }
}
