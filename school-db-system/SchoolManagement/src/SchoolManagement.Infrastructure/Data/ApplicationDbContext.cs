using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities.Common;
using SchoolManagement.Core.Entities.Master;
using SchoolManagement.Core.Entities.Tenant;
using SchoolManagement.Infrastructure.Multitenancy;

namespace SchoolManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantInfo _tenantInfo;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantInfo tenantInfo)
            : base(options)
        {
            _tenantInfo = tenantInfo;
        }

        // Master schema entities
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }

        // Common schema entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTenant> UserTenants { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Tenant schema entities
        public DbSet<Student> Students { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<StudentGuardian> StudentGuardians { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Staff> StaffMembers { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<TeacherSection> TeacherSections { get; set; }
        public DbSet<FeeType> FeeTypes { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceAllocation> ResourceAllocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Master schema entities
            modelBuilder.Entity<Tenant>().ToTable("tenants", "master");
            modelBuilder.Entity<TenantSubscription>().ToTable("tenant_subscriptions", "master");

            // Configure Common schema entities
            modelBuilder.Entity<User>().ToTable("users", "common");
            modelBuilder.Entity<Role>().ToTable("roles", "common");
            modelBuilder.Entity<Permission>().ToTable("permissions", "common");
            modelBuilder.Entity<UserRole>().ToTable("user_roles", "common");
            modelBuilder.Entity<UserTenant>().ToTable("user_tenants", "common");
            modelBuilder.Entity<RolePermission>().ToTable("role_permissions", "common");

            // Configure Tenant schema entities - schema will be determined at runtime
            if (!string.IsNullOrEmpty(_tenantInfo?.SchemaName))
            {
                // Apply tenant schema to all tenant-specific entities
                modelBuilder.Entity<Student>().ToTable("students", _tenantInfo.SchemaName);
                modelBuilder.Entity<Guardian>().ToTable("guardians", _tenantInfo.SchemaName);
                modelBuilder.Entity<StudentGuardian>().ToTable("student_guardians", _tenantInfo.SchemaName);
                modelBuilder.Entity<AcademicYear>().ToTable("academic_years", _tenantInfo.SchemaName);
                modelBuilder.Entity<Term>().ToTable("terms", _tenantInfo.SchemaName);
                modelBuilder.Entity<Grade>().ToTable("grades", _tenantInfo.SchemaName);
                modelBuilder.Entity<Section>().ToTable("sections", _tenantInfo.SchemaName);
                modelBuilder.Entity<Subject>().ToTable("subjects", _tenantInfo.SchemaName);
                modelBuilder.Entity<Enrollment>().ToTable("enrollments", _tenantInfo.SchemaName);
                modelBuilder.Entity<Attendance>().ToTable("attendance", _tenantInfo.SchemaName);
                modelBuilder.Entity<Staff>().ToTable("staff", _tenantInfo.SchemaName);
                modelBuilder.Entity<TeacherSubject>().ToTable("teacher_subjects", _tenantInfo.SchemaName);
                modelBuilder.Entity<TeacherSection>().ToTable("teacher_sections", _tenantInfo.SchemaName);
                modelBuilder.Entity<FeeType>().ToTable("fee_types", _tenantInfo.SchemaName);
                modelBuilder.Entity<FeeStructure>().ToTable("fee_structures", _tenantInfo.SchemaName);
                modelBuilder.Entity<Invoice>().ToTable("invoices", _tenantInfo.SchemaName);
                modelBuilder.Entity<InvoiceItem>().ToTable("invoice_items", _tenantInfo.SchemaName);
                modelBuilder.Entity<Payment>().ToTable("payments", _tenantInfo.SchemaName);
                modelBuilder.Entity<Announcement>().ToTable("announcements", _tenantInfo.SchemaName);
                modelBuilder.Entity<Message>().ToTable("messages", _tenantInfo.SchemaName);
                modelBuilder.Entity<Assessment>().ToTable("assessments", _tenantInfo.SchemaName);
                modelBuilder.Entity<AssessmentResult>().ToTable("assessment_results", _tenantInfo.SchemaName);
                modelBuilder.Entity<Resource>().ToTable("resources", _tenantInfo.SchemaName);
                modelBuilder.Entity<ResourceAllocation>().ToTable("resource_allocations", _tenantInfo.SchemaName);
            }

            // Configure composite keys
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserTenant>()
                .HasKey(ut => new { ut.UserId, ut.TenantId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<StudentGuardian>()
                .HasKey(sg => new { sg.StudentId, sg.GuardianId });

            modelBuilder.Entity<TeacherSubject>()
                .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

            modelBuilder.Entity<TeacherSection>()
                .HasKey(ts => new { ts.TeacherId, ts.SectionId });

            // Configure relationships and constraints
            modelBuilder.Entity<TenantSubscription>()
                .HasOne(ts => ts.Tenant)
                .WithMany()
                .HasForeignKey(ts => ts.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTenant>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTenant>()
                .HasOne(ut => ut.Tenant)
                .WithMany()
                .HasForeignKey(ut => ut.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add additional relationship configurations as needed
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Add audit trail logic
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
