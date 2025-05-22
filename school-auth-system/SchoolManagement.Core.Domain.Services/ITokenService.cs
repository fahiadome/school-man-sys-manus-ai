using System;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Domain.Services
{
    /// <summary>
    /// Interface for token service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="tenantId">The tenant ID (optional)</param>
        /// <param name="roles">The user's roles</param>
        /// <param name="permissions">The user's permissions</param>
        /// <returns>The JWT token</returns>
        string GenerateJwtToken(Core.Domain.Authentication.AuthUser user, string tenantId, IEnumerable<string> roles, IEnumerable<string> permissions);
        
        /// <summary>
        /// Validates a JWT token
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <param name="principal">The claims principal if validation succeeds</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        bool ValidateToken(string token, out System.Security.Claims.ClaimsPrincipal principal);
        
        /// <summary>
        /// Generates a refresh token
        /// </summary>
        /// <returns>The refresh token</returns>
        string GenerateRefreshToken();
    }
}
