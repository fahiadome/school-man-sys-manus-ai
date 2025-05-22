using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Implementation of tenant resolver that uses hostname and header-based resolution
    /// </summary>
    public class TenantResolver : ITenantResolver
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TenantResolver> _logger;

        public TenantResolver(
            ApplicationDbContext dbContext,
            ILogger<TenantResolver> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Resolves tenant from the current HTTP context
        /// </summary>
        public async Task<ITenantInfo> ResolveTenantAsync(HttpContext context)
        {
            // Try to resolve from X-Tenant-ID header first (for API calls)
            if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdHeader))
            {
                if (int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return await ResolveTenantByIdAsync(tenantId);
                }
            }

            // Then try to resolve from hostname
            var hostname = context.Request.Host.Host.ToLower();
            return await ResolveTenantByHostnameAsync(hostname);
        }

        private async Task<ITenantInfo> ResolveTenantByIdAsync(int tenantId)
        {
            try
            {
                var tenant = await _dbContext.Tenants
                    .Where(t => t.Id == tenantId && t.IsActive)
                    .FirstOrDefaultAsync();

                if (tenant != null)
                {
                    return new TenantInfo(
                        tenant.Id,
                        tenant.Name,
                        tenant.SchemaName,
                        tenant.ConnectionString ?? _dbContext.Database.GetConnectionString(),
                        tenant.IsActive);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant by ID {TenantId}", tenantId);
            }

            return null;
        }

        private async Task<ITenantInfo> ResolveTenantByHostnameAsync(string hostname)
        {
            try
            {
                var tenant = await _dbContext.Tenants
                    .Where(t => t.Domain == hostname && t.IsActive)
                    .FirstOrDefaultAsync();

                if (tenant != null)
                {
                    return new TenantInfo(
                        tenant.Id,
                        tenant.Name,
                        tenant.SchemaName,
                        tenant.ConnectionString ?? _dbContext.Database.GetConnectionString(),
                        tenant.IsActive);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant by hostname {Hostname}", hostname);
            }

            return null;
        }
    }
}
