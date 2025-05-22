using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.Domain.Authentication;
using SchoolManagement.Core.Domain.Repositories;
using SchoolManagement.Core.Domain.Services;

namespace SchoolManagement.Infrastructure.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IOptions<JwtSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        /// <summary>
        /// Authenticates a user with username and password
        /// </summary>
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Get user by username
            var user = await _unitOfWork.AuthUsers.GetByUsernameAsync(username);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if user is active
            if (!user.IsActive)
                throw new InvalidOperationException("User is inactive");

            // Check if user is locked out
            if (user.IsLockedOut())
                throw new InvalidOperationException("User is locked out");

            // Verify password
            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                // Record failed login attempt
                user.RecordFailedLogin(3, TimeSpan.FromMinutes(15));
                await _unitOfWork.SaveChangesAsync();
                throw new InvalidOperationException("Invalid credentials");
            }

            // Record successful login
            user.RecordSuccessfulLogin();
            
            // Get user roles and permissions
            var userWithRoles = await _unitOfWork.AuthUsers.GetWithRolesAsync(user.Id);
            var roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            var permissions = new List<string>();
            foreach (var roleId in userWithRoles.UserRoles.Select(ur => ur.RoleId))
            {
                var rolePermissions = await _unitOfWork.Permissions.GetPermissionsByRoleAsync(roleId);
                permissions.AddRange(rolePermissions.Select(p => p.Name));
            }
            
            // Generate tokens
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            
            // Save refresh token
            user.AddRefreshToken(refreshToken, refreshTokenExpiryDate, ipAddress);
            await _unitOfWork.SaveChangesAsync();
            
            // Get primary tenant if exists
            var userWithTenants = await _unitOfWork.AuthUsers.GetWithTenantsAsync(user.Id);
            var primaryTenant = userWithTenants.UserTenants.FirstOrDefault(ut => ut.IsPrimary);
            var tenantId = primaryTenant?.TenantId;
            
            // Generate JWT token
            var token = _tokenService.GenerateJwtToken(user, tenantId, roles, permissions.Distinct());
            
            return new AuthenticationResult
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Roles = roles,
                Permissions = permissions.Distinct()
            };
        }

        /// <summary>
        /// Refreshes an authentication token
        /// </summary>
        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            // Get user by refresh token
            var user = await _unitOfWork.AuthUsers.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new InvalidOperationException("Invalid refresh token");

            // Get the refresh token
            var token = user.RefreshTokens.Single(t => t.Token == refreshToken);

            // Check if token is active
            if (!token.IsActive)
                throw new InvalidOperationException("Inactive refresh token");

            // Revoke the current refresh token
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            
            user.RevokeRefreshToken(refreshToken, ipAddress, newRefreshToken);
            user.AddRefreshToken(newRefreshToken, refreshTokenExpiryDate, ipAddress);
            
            await _unitOfWork.SaveChangesAsync();
            
            // Get user roles and permissions
            var userWithRoles = await _unitOfWork.AuthUsers.GetWithRolesAsync(user.Id);
            var roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            var permissions = new List<string>();
            foreach (var roleId in userWithRoles.UserRoles.Select(ur => ur.RoleId))
            {
                var rolePermissions = await _unitOfWork.Permissions.GetPermissionsByRoleAsync(roleId);
                permissions.AddRange(rolePermissions.Select(p => p.Name));
            }
            
            // Get primary tenant if exists
            var userWithTenants = await _unitOfWork.AuthUsers.GetWithTenantsAsync(user.Id);
            var primaryTenant = userWithTenants.UserTenants.FirstOrDefault(ut => ut.IsPrimary);
            var tenantId = primaryTenant?.TenantId;
            
            // Generate JWT token
            var jwtToken = _tokenService.GenerateJwtToken(user, tenantId, roles, permissions.Distinct());
            
            return new AuthenticationResult
            {
                UserId = user.Id,
                Username = user.Username,
                Token = jwtToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Roles = roles,
                Permissions = permissions.Distinct()
            };
        }

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        public async Task RevokeTokenAsync(string refreshToken, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            // Get user by refresh token
            var user = await _unitOfWork.AuthUsers.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new InvalidOperationException("Invalid refresh token");

            // Revoke the refresh token
            user.RevokeRefreshToken(refreshToken, ipAddress);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        public async Task<string> RegisterUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Check if username is already taken
            var existingUserByUsername = await _unitOfWork.AuthUsers.GetByUsernameAsync(username);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("Username is already taken");

            // Check if email is already taken
            var existingUserByEmail = await _unitOfWork.AuthUsers.GetByEmailAsync(email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("Email is already taken");

            // Hash password
            var (passwordHash, salt) = _passwordHasher.HashPassword(password);

            // Create user
            var user = new AuthUser(username, email, passwordHash, salt);
            await _unitOfWork.AuthUsers.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ArgumentException("Current password cannot be empty", nameof(currentPassword));

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password cannot be empty", nameof(newPassword));

            // Get user
            var user = await _unitOfWork.AuthUsers.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Verify current password
            if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.Salt))
                throw new InvalidOperationException("Current password is incorrect");

            // Hash new password
            var (passwordHash, salt) = _passwordHasher.HashPassword(newPassword);

            // Update password
            user.UpdatePassword(passwordHash, salt);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        public async Task AssignRoleAsync(string userId, string roleId, string tenantId = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

            // Get user
            var user = await _unitOfWork.AuthUsers.GetWithRolesAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Get role
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException("Role not found");

            // Check if user already has the role
            if (user.UserRoles.Any(ur => ur.RoleId == roleId && (tenantId == null || ur.TenantId == tenantId)))
                return;

            // Add role to user
            var userRole = new UserRole(userId, roleId, tenantId);
            user.UserRoles.Add(userRole);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a role from a user
        /// </summary>
        public async Task RemoveRoleAsync(string userId, string roleId, string tenantId = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

            // Get user
            var user = await _unitOfWork.AuthUsers.GetWithRolesAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Find user role
            var userRole = user.UserRoles.FirstOrDefault(ur => 
                ur.RoleId == roleId && (tenantId == null || ur.TenantId == tenantId));
            
            if (userRole == null)
                return;

            // Remove role from user
            user.UserRoles.Remove(userRole);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Assigns a user to a tenant
        /// </summary>
        public async Task AssignTenantAsync(string userId, string tenantId, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            // Get user
            var user = await _unitOfWork.AuthUsers.GetWithTenantsAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Check if user is already assigned to the tenant
            if (user.UserTenants.Any(ut => ut.TenantId == tenantId))
                return;

            // If this is the primary tenant, unset any existing primary tenant
            if (isPrimary)
            {
                foreach (var userTenant in user.UserTenants.Where(ut => ut.IsPrimary))
                {
                    userTenant.IsPrimary = false;
                }
            }

            // Add tenant to user
            var userTenant = new UserTenant(userId, tenantId, isPrimary);
            user.UserTenants.Add(userTenant);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user from a tenant
        /// </summary>
        public async Task RemoveTenantAsync(string userId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            // Get user
            var user = await _unitOfWork.AuthUsers.GetWithTenantsAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Find user tenant
            var userTenant = user.UserTenants.FirstOrDefault(ut => ut.TenantId == tenantId);
            if (userTenant == null)
                return;

            // Remove tenant from user
            user.UserTenants.Remove(userTenant);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
