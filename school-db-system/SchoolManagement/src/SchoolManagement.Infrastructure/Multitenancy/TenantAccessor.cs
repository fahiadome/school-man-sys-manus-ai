using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Accessor for tenant information from the current HTTP context
    /// </summary>
    public class TenantAccessor : ITenantInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ITenantInfo _currentTenant;

        public TenantAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current tenant ID
        /// </summary>
        public int Id => GetTenantInfo()?.Id ?? 0;

        /// <summary>
        /// Gets the current tenant name
        /// </summary>
        public string Name => GetTenantInfo()?.Name ?? string.Empty;

        /// <summary>
        /// Gets the current tenant schema name
        /// </summary>
        public string SchemaName => GetTenantInfo()?.SchemaName ?? string.Empty;

        /// <summary>
        /// Gets the current tenant connection string
        /// </summary>
        public string ConnectionString => GetTenantInfo()?.ConnectionString ?? string.Empty;

        /// <summary>
        /// Gets whether the current tenant is active
        /// </summary>
        public bool IsActive => GetTenantInfo()?.IsActive ?? false;

        private ITenantInfo GetTenantInfo()
        {
            if (_currentTenant != null)
            {
                return _currentTenant;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Items.TryGetValue("TenantInfo", out var tenantObj))
            {
                _currentTenant = tenantObj as ITenantInfo;
                return _currentTenant;
            }

            return null;
        }
    }
}
