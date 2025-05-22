using System.Threading.Tasks;
using SchoolManagement.Core.Domain.Authentication;

namespace SchoolManagement.Core.Domain.Repositories
{
    /// <summary>
    /// Repository interface for role operations
    /// </summary>
    public interface IRoleRepository : IGenericRepository<Role>
    {
        /// <summary>
        /// Gets a role by name
        /// </summary>
        /// <param name="name">The role name</param>
        /// <returns>The role if found, null otherwise</returns>
        Task<Role> GetByNameAsync(string name);
        
        /// <summary>
        /// Gets a role with permissions
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>The role with permissions if found, null otherwise</returns>
        Task<Role> GetWithPermissionsAsync(string roleId);
        
        /// <summary>
        /// Gets roles for a specific tenant
        /// </summary>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>Collection of roles for the tenant</returns>
        Task<IEnumerable<Role>> GetRolesByTenantAsync(string tenantId);
        
        /// <summary>
        /// Gets roles for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of roles for the user</returns>
        Task<IEnumerable<Role>> GetRolesByUserAsync(string userId);
        
        /// <summary>
        /// Gets roles for a specific user in a specific tenant
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>Collection of roles for the user in the tenant</returns>
        Task<IEnumerable<Role>> GetRolesByUserAndTenantAsync(string userId, string tenantId);
    }
}
