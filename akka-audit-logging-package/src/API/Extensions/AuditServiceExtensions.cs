using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Services;
using SchoolManagement.Infrastructure.Actors;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Application.Services;
using System;

namespace SchoolManagement.API.Extensions
{
    /// <summary>
    /// Extension methods for registering audit logging services with dependency injection
    /// </summary>
    public static class AuditServiceExtensions
    {
        /// <summary>
        /// Add Akka.NET-based audit logging services to the service collection
        /// </summary>
        public static IServiceCollection AddAkkaAuditServices(this IServiceCollection services)
        {
            // Register actor system factory as singleton
            services.AddSingleton<ActorSystemFactory>();
            
            // Register audit repository
            services.AddScoped<IAuditRepository, AuditRepository>();
            
            // Register audit service
            services.AddScoped<IAuditService, AkkaAuditService>();
            
            // Add required packages
            services.AddLogging();
            
            return services;
        }
        
        /// <summary>
        /// Configure audit logging connection string
        /// </summary>
        public static IServiceCollection ConfigureAuditLogging(this IServiceCollection services, string connectionString)
        {
            // Store connection string in configuration
            services.Configure<AuditLoggingOptions>(options => 
            {
                options.ConnectionString = connectionString;
            });
            
            return services;
        }
    }
    
    /// <summary>
    /// Options for audit logging configuration
    /// </summary>
    public class AuditLoggingOptions
    {
        /// <summary>
        /// Connection string for audit database
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Maximum events per second per tenant (default: 1000)
        /// </summary>
        public int MaxEventsPerSecond { get; set; } = 1000;
        
        /// <summary>
        /// Maximum burst size per tenant (default: 2000)
        /// </summary>
        public int BurstSize { get; set; } = 2000;
        
        /// <summary>
        /// Batch size for database operations (default: 100)
        /// </summary>
        public int BatchSize { get; set; } = 100;
        
        /// <summary>
        /// Batch flush interval in seconds (default: 2)
        /// </summary>
        public int BatchFlushIntervalSeconds { get; set; } = 2;
    }
}
