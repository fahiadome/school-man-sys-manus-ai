using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Core.Domain.Authentication;
using SchoolManagement.Infrastructure.Multitenancy;

namespace SchoolManagement.Infrastructure.Data
{
    /// <summary>
    /// Application database context with multi-tenant support
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantInfo _tenantInfo;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantInfo tenantInfo)
            : base(options)
        {
            _tenantInfo = tenantInfo;
        }

        // Authentication entities
        public DbSet<AuthUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTenant> UserTenants { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Multi-tenant entities
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure common schema entities (shared across all tenants)
            ConfigureCommonSchema(modelBuilder);

            // Configure master schema entities (tenant management)
            ConfigureMasterSchema(modelBuilder);

            // Configure tenant-specific schema entities
            if (!string.IsNullOrEmpty(_tenantInfo?.SchemaName))
            {
                ConfigureTenantSchema(modelBuilder, _tenantInfo.SchemaName);
            }
        }

        private void ConfigureCommonSchema(ModelBuilder modelBuilder)
        {
            // AuthUser entity
            modelBuilder.Entity<AuthUser>(entity =>
            {
                entity.ToTable("users", "common");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Salt).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles", "common");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Permission entity
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions", "common");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.Resource }).IsUnique();
            });

            // UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles", "common");
                entity.HasKey(e => new { e.UserId, e.RoleId });
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserTenant entity
            modelBuilder.Entity<UserTenant>(entity =>
            {
                entity.ToTable("user_tenants", "common");
                entity.HasKey(e => new { e.UserId, e.TenantId });
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserTenants)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RolePermission entity
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions", "common");
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Permission)
                    .WithMany()
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens", "common");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Token).IsRequired().HasMaxLength(128);
                entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RevokedByIp).HasMaxLength(50);
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureMasterSchema(ModelBuilder modelBuilder)
        {
            // Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("tenants", "master");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SchemaName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Domain).HasMaxLength(100);
                entity.HasIndex(e => e.SchemaName).IsUnique();
                entity.HasIndex(e => e.Domain).IsUnique();
            });
        }

        private void ConfigureTenantSchema(ModelBuilder modelBuilder, string schema)
        {
            // Configure tenant-specific entities here if needed
            // For this authentication implementation, we don't have tenant-specific authentication entities
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Set schema search path for PostgreSQL
            if (!string.IsNullOrEmpty(_tenantInfo?.SchemaName))
            {
                await Database.ExecuteSqlRawAsync($"SET search_path TO {_tenantInfo.SchemaName}, common, master, public;", cancellationToken);
            }
            else
            {
                await Database.ExecuteSqlRawAsync("SET search_path TO common, master, public;", cancellationToken);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
