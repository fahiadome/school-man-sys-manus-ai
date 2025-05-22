using System.Threading.Tasks;
using SchoolManagement.Core.Domain.Authentication;

namespace SchoolManagement.Core.Domain.Repositories
{
    /// <summary>
    /// Repository interface for authentication user operations
    /// </summary>
    public interface IAuthUserRepository : IGenericRepository<AuthUser>
    {
        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<AuthUser> GetByUsernameAsync(string username);
        
        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<AuthUser> GetByEmailAsync(string email);
        
        /// <summary>
        /// Gets a user with roles
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user with roles if found, null otherwise</returns>
        Task<AuthUser> GetWithRolesAsync(string userId);
        
        /// <summary>
        /// Gets a user with tenants
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user with tenants if found, null otherwise</returns>
        Task<AuthUser> GetWithTenantsAsync(string userId);
        
        /// <summary>
        /// Gets a user with refresh tokens
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user with refresh tokens if found, null otherwise</returns>
        Task<AuthUser> GetWithRefreshTokensAsync(string userId);
        
        /// <summary>
        /// Gets a user by refresh token
        /// </summary>
        /// <param name="refreshToken">The refresh token</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<AuthUser> GetByRefreshTokenAsync(string refreshToken);
        
        /// <summary>
        /// Gets a user with all related data (roles, tenants, refresh tokens)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user with all related data if found, null otherwise</returns>
        Task<AuthUser> GetCompleteUserAsync(string userId);
    }
}
