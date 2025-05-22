using System.Threading.Tasks;
using SchoolManagement.Core.Domain.Authentication;

namespace SchoolManagement.Core.Domain.Repositories
{
    /// <summary>
    /// Repository interface for permission operations
    /// </summary>
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        /// <summary>
        /// Gets a permission by name
        /// </summary>
        /// <param name="name">The permission name</param>
        /// <returns>The permission if found, null otherwise</returns>
        Task<Permission> GetByNameAsync(string name);
        
        /// <summary>
        /// Gets permissions by resource
        /// </summary>
        /// <param name="resource">The resource name</param>
        /// <returns>Collection of permissions for the resource</returns>
        Task<IEnumerable<Permission>> GetByResourceAsync(string resource);
        
        /// <summary>
        /// Gets permissions for a specific role
        /// </summary>
        /// <param name="roleId">The role ID</param>
        /// <returns>Collection of permissions for the role</returns>
        Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(string roleId);
        
        /// <summary>
        /// Gets permissions for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of permissions for the user</returns>
        Task<IEnumerable<Permission>> GetPermissionsByUserAsync(string userId);
        
        /// <summary>
        /// Gets permissions for a specific user in a specific tenant
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>Collection of permissions for the user in the tenant</returns>
        Task<IEnumerable<Permission>> GetPermissionsByUserAndTenantAsync(string userId, string tenantId);
    }
}
