using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Refresh token entity for JWT authentication
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the refresh token ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the token value
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the IP address that created the token
        /// </summary>
        public string CreatedByIp { get; set; }

        /// <summary>
        /// Gets or sets the revocation date
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Gets or sets the IP address that revoked the token
        /// </summary>
        public string RevokedByIp { get; set; }

        /// <summary>
        /// Gets or sets the replacement token
        /// </summary>
        public string ReplacedByToken { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public virtual AuthUser User { get; set; }

        /// <summary>
        /// Gets a value indicating whether the token is expired
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

        /// <summary>
        /// Gets a value indicating whether the token is active
        /// </summary>
        public bool IsActive => !RevokedAt.HasValue && !IsExpired;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshToken"/> class
        /// </summary>
        public RefreshToken()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes the token
        /// </summary>
        public void Revoke(string ipAddress, string replacementToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress;
            ReplacedByToken = replacementToken;
        }
    }
}
