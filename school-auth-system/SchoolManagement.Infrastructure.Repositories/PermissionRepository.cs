using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Domain.Authentication;
using SchoolManagement.Core.Domain.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for permission operations
    /// </summary>
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a permission by name
        /// </summary>
        public async Task<Permission> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Permission name cannot be empty", nameof(name));

            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        /// <summary>
        /// Gets permissions by resource
        /// </summary>
        public async Task<IEnumerable<Permission>> GetByResourceAsync(string resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be empty", nameof(resource));

            return await _context.Permissions
                .Where(p => p.Resource == resource)
                .ToListAsync();
        }

        /// <summary>
        /// Gets permissions for a specific role
        /// </summary>
        public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        /// <summary>
        /// Gets permissions for a specific user
        /// </summary>
        public async Task<IEnumerable<Permission>> GetPermissionsByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Gets permissions for a specific user in a specific tenant
        /// </summary>
        public async Task<IEnumerable<Permission>> GetPermissionsByUserAndTenantAsync(string userId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }
    }
}
