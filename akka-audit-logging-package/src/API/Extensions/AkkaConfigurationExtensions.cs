using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolManagement.Infrastructure.Actors;
using System;

namespace SchoolManagement.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring Akka.NET in the application
    /// </summary>
    public static class AkkaConfigurationExtensions
    {
        /// <summary>
        /// Add Akka.NET configuration to the application
        /// </summary>
        public static IServiceCollection AddAkkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("AuditDatabase");
            
            // Configure audit logging
            services.ConfigureAuditLogging(connectionString);
            
            // Add Akka.NET packages
            services.AddAkkaPackages();
            
            return services;
        }
        
        /// <summary>
        /// Add required Akka.NET packages
        /// </summary>
        private static IServiceCollection AddAkkaPackages(this IServiceCollection services)
        {
            // Add Akka.NET logging integration
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            
            return services;
        }
        
        /// <summary>
        /// Initialize the Akka.NET actor system during application startup
        /// </summary>
        public static IApplicationBuilder UseAkkaActorSystem(this IApplicationBuilder app)
        {
            // Initialize actor system
            var actorSystemFactory = app.ApplicationServices.GetRequiredService<ActorSystemFactory>();
            
            // Register application lifetime hooks
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            
            lifetime.ApplicationStopping.Register(() =>
            {
                // Dispose actor system on application shutdown
                (actorSystemFactory as IDisposable)?.Dispose();
            });
            
            return app;
        }
    }
}
