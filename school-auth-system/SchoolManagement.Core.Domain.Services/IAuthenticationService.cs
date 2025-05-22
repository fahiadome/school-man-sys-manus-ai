using System;
using System.Threading.Tasks;
using SchoolManagement.Core.Domain.Authentication;
using SchoolManagement.Core.Domain.Repositories;

namespace SchoolManagement.Core.Domain.Services
{
    /// <summary>
    /// Interface for authentication service
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="ipAddress">The IP address of the client</param>
        /// <returns>Authentication result with token information</returns>
        Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress);
        
        /// <summary>
        /// Refreshes an authentication token
        /// </summary>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="ipAddress">The IP address of the client</param>
        /// <returns>Authentication result with new token information</returns>
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string ipAddress);
        
        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        /// <param name="refreshToken">The refresh token</param>
        /// <param name="ipAddress">The IP address of the client</param>
        Task RevokeTokenAsync(string refreshToken, string ipAddress);
        
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="email">The email address</param>
        /// <param name="password">The password</param>
        /// <returns>The ID of the newly created user</returns>
        Task<string> RegisterUserAsync(string username, string email, string password);
        
        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="currentPassword">The current password</param>
        /// <param name="newPassword">The new password</param>
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        
        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="roleId">The role ID</param>
        /// <param name="tenantId">The tenant ID (optional)</param>
        Task AssignRoleAsync(string userId, string roleId, string tenantId = null);
        
        /// <summary>
        /// Removes a role from a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="roleId">The role ID</param>
        /// <param name="tenantId">The tenant ID (optional)</param>
        Task RemoveRoleAsync(string userId, string roleId, string tenantId = null);
        
        /// <summary>
        /// Assigns a user to a tenant
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="tenantId">The tenant ID</param>
        /// <param name="isPrimary">Whether this is the primary tenant for the user</param>
        Task AssignTenantAsync(string userId, string tenantId, bool isPrimary = false);
        
        /// <summary>
        /// Removes a user from a tenant
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="tenantId">The tenant ID</param>
        Task RemoveTenantAsync(string userId, string tenantId);
    }

    /// <summary>
    /// Authentication result
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Gets or sets the JWT token
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }
        
        /// <summary>
        /// Gets or sets the token expiration time
        /// </summary>
        public DateTime TokenExpiration { get; set; }
        
        /// <summary>
        /// Gets or sets the roles
        /// </summary>
        public IEnumerable<string> Roles { get; set; }
        
        /// <summary>
        /// Gets or sets the permissions
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }
    }
}
