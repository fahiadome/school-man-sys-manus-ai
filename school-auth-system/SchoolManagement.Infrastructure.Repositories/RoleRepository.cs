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
    /// Repository implementation for role operations
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a role by name
        /// </summary>
        public async Task<Role> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        /// <summary>
        /// Gets a role with permissions
        /// </summary>
        public async Task<Role> GetWithPermissionsAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        /// <summary>
        /// Gets roles for a specific tenant
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesByTenantAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            return await _context.UserRoles
                .Where(ur => ur.TenantId == tenantId)
                .Select(ur => ur.Role)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Gets roles for a specific user
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesByUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Gets roles for a specific user in a specific tenant
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesByUserAndTenantAsync(string userId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }
    }
}
