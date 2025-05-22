using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Interface for resolving tenant from the current request
    /// </summary>
    public interface ITenantResolver
    {
        /// <summary>
        /// Resolves tenant from the current HTTP context
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>The tenant info if resolved, null otherwise</returns>
        Task<ITenantInfo> ResolveTenantAsync(HttpContext context);
    }
}
