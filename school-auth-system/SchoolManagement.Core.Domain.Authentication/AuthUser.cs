using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Authentication user entity
    /// </summary>
    public class AuthUser
    {
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password hash
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email is confirmed
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the lockout end date
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets the last login date
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the user tenants
        /// </summary>
        public virtual ICollection<UserTenant> UserTenants { get; set; }

        /// <summary>
        /// Gets or sets the refresh tokens
        /// </summary>
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthUser"/> class
        /// </summary>
        public AuthUser()
        {
            UserRoles = new List<UserRole>();
            UserTenants = new List<UserTenant>();
            RefreshTokens = new List<RefreshToken>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthUser"/> class
        /// </summary>
        public AuthUser(string username, string email, string passwordHash, string salt)
            : this()
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
        }

        /// <summary>
        /// Confirms the email address
        /// </summary>
        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the password
        /// </summary>
        public void UpdatePassword(string passwordHash, string salt)
        {
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Records a successful login
        /// </summary>
        public void RecordSuccessfulLogin()
        {
            LastLogin = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Records a failed login
        /// </summary>
        public void RecordFailedLogin(int maxAttempts, TimeSpan lockoutDuration)
        {
            FailedLoginAttempts++;
            
            if (FailedLoginAttempts >= maxAttempts)
            {
                LockoutEnd = DateTime.UtcNow.Add(lockoutDuration);
            }
            
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if the user is locked out
        /// </summary>
        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }

        /// <summary>
        /// Activates the user
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Deactivates the user
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a refresh token
        /// </summary>
        public void AddRefreshToken(string token, DateTime expiryDate, string createdByIp)
        {
            RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                ExpiryDate = expiryDate,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = createdByIp,
                UserId = Id
            });
            
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        public void RevokeRefreshToken(string token, string revokedByIp, string replacedByToken = null)
        {
            var refreshToken = RefreshTokens.FirstOrDefault(t => t.Token == token);
            
            if (refreshToken != null)
            {
                refreshToken.Revoke(revokedByIp, replacedByToken);
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
