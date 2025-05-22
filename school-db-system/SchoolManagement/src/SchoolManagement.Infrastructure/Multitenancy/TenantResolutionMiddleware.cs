using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Middleware for resolving tenant from the current request
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(
            RequestDelegate next,
            ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantResolver resolver)
        {
            try
            {
                // Resolve tenant from the request
                var tenant = await resolver.ResolveTenantAsync(context);
                
                if (tenant != null)
                {
                    // Store tenant info in the HTTP context for the request duration
                    context.Items["TenantInfo"] = tenant;
                    
                    // Register tenant info in the DI container for the request scope
                    context.RequestServices
                        .GetRequiredService<IHttpContextAccessor>()
                        .HttpContext = context;
                }
                else
                {
                    _logger.LogWarning("No tenant resolved for the request");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant");
            }

            // Continue with the request pipeline
            await _next(context);
        }
    }
}
