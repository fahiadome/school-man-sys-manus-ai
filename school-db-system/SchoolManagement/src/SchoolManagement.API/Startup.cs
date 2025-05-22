using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Multitenancy;

namespace SchoolManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add HTTP context accessor
            services.AddHttpContextAccessor();

            // Add controllers
            services.AddControllers();

            // Add PostgreSQL database context
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var tenantInfo = serviceProvider.GetService<ITenantInfo>();
                var connectionString = Configuration.GetConnectionString("DefaultConnection");

                options.UseNpgsql(connectionString);
            });

            // Add multi-tenancy services
            services.AddScoped<ITenantResolver, TenantResolver>();
            services.AddScoped<ITenantInfo, TenantAccessor>();

            // Add tenant migration service
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<TenantMigrationService>>();
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                return new TenantMigrationService(logger, connectionString);
            });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "School Management API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TenantMigrationService migrationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Apply base migrations
            migrationService.MigrateBaseSchemaAsync().Wait();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Add tenant resolution middleware
            app.UseMiddleware<TenantResolutionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
