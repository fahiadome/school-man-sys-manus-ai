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
    /// Repository implementation for authentication user operations
    /// </summary>
    public class AuthUserRepository : GenericRepository<AuthUser>, IAuthUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a user by username
        /// </summary>
        public async Task<AuthUser> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        public async Task<AuthUser> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Gets a user with roles
        /// </summary>
        public async Task<AuthUser> GetWithRolesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Gets a user with tenants
        /// </summary>
        public async Task<AuthUser> GetWithTenantsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.Users
                .Include(u => u.UserTenants)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Gets a user with refresh tokens
        /// </summary>
        public async Task<AuthUser> GetWithRefreshTokensAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Gets a user by refresh token
        /// </summary>
        public async Task<AuthUser> GetByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        }

        /// <summary>
        /// Gets a user with all related data (roles, tenants, refresh tokens)
        /// </summary>
        public async Task<AuthUser> GetCompleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserTenants)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
