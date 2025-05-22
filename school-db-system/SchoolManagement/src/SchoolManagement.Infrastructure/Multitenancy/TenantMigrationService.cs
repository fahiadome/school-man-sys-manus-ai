using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Core.Entities.Master;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Service for managing tenant provisioning and migrations
    /// </summary>
    public class TenantMigrationService
    {
        private readonly ILogger<TenantMigrationService> _logger;
        private readonly string _connectionString;

        public TenantMigrationService(
            ILogger<TenantMigrationService> logger,
            string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        /// <summary>
        /// Applies migrations for master and common schemas
        /// </summary>
        public async Task MigrateBaseSchemaAsync()
        {
            try
            {
                _logger.LogInformation("Applying migrations for master and common schemas");

                // Create options for the DbContext
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(_connectionString)
                    .Options;

                // Create a temporary tenant info for the migration
                var tenantInfo = new TenantInfo(0, "System", "public", _connectionString, true);

                // Create and migrate the DbContext
                using (var context = new ApplicationDbContext(options, tenantInfo))
                {
                    // Ensure database exists
                    await context.Database.EnsureCreatedAsync();

                    // Create master schema if it doesn't exist
                    await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS master;");

                    // Create common schema if it doesn't exist
                    await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS common;");

                    // Apply migrations
                    await context.Database.MigrateAsync();
                }

                _logger.LogInformation("Successfully applied migrations for master and common schemas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying migrations for master and common schemas");
                throw;
            }
        }

        /// <summary>
        /// Provisions a new tenant with its own schema
        /// </summary>
        /// <param name="tenant">The tenant to provision</param>
        public async Task ProvisionTenantAsync(Tenant tenant)
        {
            try
            {
                _logger.LogInformation("Provisioning tenant {TenantId} ({TenantName})", tenant.Id, tenant.Name);

                // Create options for the DbContext
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(tenant.ConnectionString ?? _connectionString)
                    .Options;

                // Create a tenant info for the migration
                var tenantInfo = new TenantInfo(
                    tenant.Id,
                    tenant.Name,
                    tenant.SchemaName,
                    tenant.ConnectionString ?? _connectionString,
                    tenant.IsActive);

                // Create and migrate the DbContext
                using (var context = new ApplicationDbContext(options, tenantInfo))
                {
                    // Create tenant schema if it doesn't exist
                    await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS {tenant.SchemaName};");

                    // Apply tenant-specific migrations
                    await ApplyTenantMigrationsAsync(context, tenant.SchemaName);

                    // Initialize tenant with required data
                    await InitializeTenantDataAsync(context, tenant);
                }

                _logger.LogInformation("Successfully provisioned tenant {TenantId} ({TenantName})", tenant.Id, tenant.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error provisioning tenant {TenantId} ({TenantName})", tenant.Id, tenant.Name);
                throw;
            }
        }

        /// <summary>
        /// Applies migrations for a specific tenant schema
        /// </summary>
        private async Task ApplyTenantMigrationsAsync(ApplicationDbContext context, string schemaName)
        {
            // Set search path to the tenant schema
            await context.Database.ExecuteSqlRawAsync($"SET search_path TO {schemaName}, public;");

            // Create tables for tenant-specific entities
            var sql = @"
                -- Create grades table
                CREATE TABLE IF NOT EXISTS grades (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(50) NOT NULL,
                    display_name VARCHAR(50) NOT NULL,
                    sequence INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create sections table
                CREATE TABLE IF NOT EXISTS sections (
                    id SERIAL PRIMARY KEY,
                    grade_id INT NOT NULL,
                    name VARCHAR(50) NOT NULL,
                    capacity INT NOT NULL,
                    room VARCHAR(50) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_sections_grade FOREIGN KEY (grade_id) REFERENCES grades (id) ON DELETE CASCADE
                );

                -- Create academic_years table
                CREATE TABLE IF NOT EXISTS academic_years (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(50) NOT NULL,
                    start_date DATE NOT NULL,
                    end_date DATE NOT NULL,
                    is_current BOOLEAN NOT NULL DEFAULT FALSE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create terms table
                CREATE TABLE IF NOT EXISTS terms (
                    id SERIAL PRIMARY KEY,
                    academic_year_id INT NOT NULL,
                    name VARCHAR(50) NOT NULL,
                    start_date DATE NOT NULL,
                    end_date DATE NOT NULL,
                    is_current BOOLEAN NOT NULL DEFAULT FALSE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_terms_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id) ON DELETE CASCADE
                );

                -- Create subjects table
                CREATE TABLE IF NOT EXISTS subjects (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    code VARCHAR(20) NOT NULL,
                    description VARCHAR(255) NULL,
                    is_active BOOLEAN NOT NULL DEFAULT TRUE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create students table
                CREATE TABLE IF NOT EXISTS students (
                    id SERIAL PRIMARY KEY,
                    user_id INT NOT NULL,
                    admission_number VARCHAR(50) NOT NULL,
                    date_of_birth DATE NOT NULL,
                    gender VARCHAR(10) NOT NULL,
                    address VARCHAR(255) NULL,
                    nationality VARCHAR(50) NULL,
                    religion VARCHAR(50) NULL,
                    admission_date DATE NOT NULL,
                    current_grade_id INT NULL,
                    current_section_id INT NULL,
                    status VARCHAR(20) NOT NULL DEFAULT 'Active',
                    photo_url VARCHAR(255) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_students_grade FOREIGN KEY (current_grade_id) REFERENCES grades (id),
                    CONSTRAINT fk_students_section FOREIGN KEY (current_section_id) REFERENCES sections (id)
                );

                -- Create guardians table
                CREATE TABLE IF NOT EXISTS guardians (
                    id SERIAL PRIMARY KEY,
                    user_id INT NOT NULL,
                    relationship VARCHAR(50) NOT NULL,
                    occupation VARCHAR(100) NULL,
                    work_address VARCHAR(255) NULL,
                    education_level VARCHAR(50) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create student_guardians table
                CREATE TABLE IF NOT EXISTS student_guardians (
                    student_id INT NOT NULL,
                    guardian_id INT NOT NULL,
                    is_primary BOOLEAN NOT NULL DEFAULT FALSE,
                    can_pickup BOOLEAN NOT NULL DEFAULT FALSE,
                    emergency_contact BOOLEAN NOT NULL DEFAULT FALSE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (student_id, guardian_id),
                    CONSTRAINT fk_student_guardians_student FOREIGN KEY (student_id) REFERENCES students (id) ON DELETE CASCADE,
                    CONSTRAINT fk_student_guardians_guardian FOREIGN KEY (guardian_id) REFERENCES guardians (id) ON DELETE CASCADE
                );

                -- Create enrollments table
                CREATE TABLE IF NOT EXISTS enrollments (
                    id SERIAL PRIMARY KEY,
                    student_id INT NOT NULL,
                    academic_year_id INT NOT NULL,
                    term_id INT NOT NULL,
                    grade_id INT NOT NULL,
                    section_id INT NULL,
                    enrollment_date DATE NOT NULL,
                    status VARCHAR(20) NOT NULL DEFAULT 'Active',
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_enrollments_student FOREIGN KEY (student_id) REFERENCES students (id) ON DELETE CASCADE,
                    CONSTRAINT fk_enrollments_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id),
                    CONSTRAINT fk_enrollments_term FOREIGN KEY (term_id) REFERENCES terms (id),
                    CONSTRAINT fk_enrollments_grade FOREIGN KEY (grade_id) REFERENCES grades (id),
                    CONSTRAINT fk_enrollments_section FOREIGN KEY (section_id) REFERENCES sections (id)
                );

                -- Create attendance table
                CREATE TABLE IF NOT EXISTS attendance (
                    id SERIAL PRIMARY KEY,
                    student_id INT NOT NULL,
                    date DATE NOT NULL,
                    status VARCHAR(20) NOT NULL,
                    reason VARCHAR(255) NULL,
                    recorded_by INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_attendance_student FOREIGN KEY (student_id) REFERENCES students (id) ON DELETE CASCADE
                );

                -- Create staff table
                CREATE TABLE IF NOT EXISTS staff (
                    id SERIAL PRIMARY KEY,
                    user_id INT NOT NULL,
                    employee_id VARCHAR(50) NOT NULL,
                    date_of_birth DATE NOT NULL,
                    gender VARCHAR(10) NOT NULL,
                    address VARCHAR(255) NULL,
                    joining_date DATE NOT NULL,
                    designation VARCHAR(100) NOT NULL,
                    department VARCHAR(100) NULL,
                    qualification VARCHAR(255) NULL,
                    experience INT NOT NULL DEFAULT 0,
                    status VARCHAR(20) NOT NULL DEFAULT 'Active',
                    photo_url VARCHAR(255) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create teacher_subjects table
                CREATE TABLE IF NOT EXISTS teacher_subjects (
                    teacher_id INT NOT NULL,
                    subject_id INT NOT NULL,
                    academic_year_id INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (teacher_id, subject_id),
                    CONSTRAINT fk_teacher_subjects_teacher FOREIGN KEY (teacher_id) REFERENCES staff (id) ON DELETE CASCADE,
                    CONSTRAINT fk_teacher_subjects_subject FOREIGN KEY (subject_id) REFERENCES subjects (id) ON DELETE CASCADE,
                    CONSTRAINT fk_teacher_subjects_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id)
                );

                -- Create teacher_sections table
                CREATE TABLE IF NOT EXISTS teacher_sections (
                    teacher_id INT NOT NULL,
                    section_id INT NOT NULL,
                    academic_year_id INT NOT NULL,
                    is_class_teacher BOOLEAN NOT NULL DEFAULT FALSE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (teacher_id, section_id),
                    CONSTRAINT fk_teacher_sections_teacher FOREIGN KEY (teacher_id) REFERENCES staff (id) ON DELETE CASCADE,
                    CONSTRAINT fk_teacher_sections_section FOREIGN KEY (section_id) REFERENCES sections (id) ON DELETE CASCADE,
                    CONSTRAINT fk_teacher_sections_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id)
                );

                -- Create fee_types table
                CREATE TABLE IF NOT EXISTS fee_types (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    description VARCHAR(255) NULL,
                    frequency VARCHAR(20) NOT NULL,
                    is_mandatory BOOLEAN NOT NULL DEFAULT FALSE,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create fee_structures table
                CREATE TABLE IF NOT EXISTS fee_structures (
                    id SERIAL PRIMARY KEY,
                    fee_type_id INT NOT NULL,
                    grade_id INT NOT NULL,
                    academic_year_id INT NOT NULL,
                    term_id INT NULL,
                    amount DECIMAL(18,2) NOT NULL,
                    due_date DATE NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_fee_structures_fee_type FOREIGN KEY (fee_type_id) REFERENCES fee_types (id) ON DELETE CASCADE,
                    CONSTRAINT fk_fee_structures_grade FOREIGN KEY (grade_id) REFERENCES grades (id),
                    CONSTRAINT fk_fee_structures_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id),
                    CONSTRAINT fk_fee_structures_term FOREIGN KEY (term_id) REFERENCES terms (id)
                );

                -- Create invoices table
                CREATE TABLE IF NOT EXISTS invoices (
                    id SERIAL PRIMARY KEY,
                    student_id INT NOT NULL,
                    academic_year_id INT NOT NULL,
                    term_id INT NULL,
                    invoice_number VARCHAR(50) NOT NULL,
                    issue_date DATE NOT NULL,
                    due_date DATE NOT NULL,
                    total_amount DECIMAL(18,2) NOT NULL,
                    discount DECIMAL(18,2) NOT NULL DEFAULT 0,
                    net_amount DECIMAL(18,2) NOT NULL,
                    status VARCHAR(20) NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_invoices_student FOREIGN KEY (student_id) REFERENCES students (id) ON DELETE CASCADE,
                    CONSTRAINT fk_invoices_academic_year FOREIGN KEY (academic_year_id) REFERENCES academic_years (id),
                    CONSTRAINT fk_invoices_term FOREIGN KEY (term_id) REFERENCES terms (id)
                );

                -- Create invoice_items table
                CREATE TABLE IF NOT EXISTS invoice_items (
                    id SERIAL PRIMARY KEY,
                    invoice_id INT NOT NULL,
                    fee_structure_id INT NOT NULL,
                    amount DECIMAL(18,2) NOT NULL,
                    discount DECIMAL(18,2) NOT NULL DEFAULT 0,
                    net_amount DECIMAL(18,2) NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_invoice_items_invoice FOREIGN KEY (invoice_id) REFERENCES invoices (id) ON DELETE CASCADE,
                    CONSTRAINT fk_invoice_items_fee_structure FOREIGN KEY (fee_structure_id) REFERENCES fee_structures (id)
                );

                -- Create payments table
                CREATE TABLE IF NOT EXISTS payments (
                    id SERIAL PRIMARY KEY,
                    invoice_id INT NOT NULL,
                    payment_date DATE NOT NULL,
                    amount DECIMAL(18,2) NOT NULL,
                    payment_method VARCHAR(50) NOT NULL,
                    reference_number VARCHAR(100) NULL,
                    received_by INT NOT NULL,
                    remarks VARCHAR(255) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_payments_invoice FOREIGN KEY (invoice_id) REFERENCES invoices (id) ON DELETE CASCADE
                );

                -- Create assessments table
                CREATE TABLE IF NOT EXISTS assessments (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    description VARCHAR(255) NULL,
                    assessment_type VARCHAR(50) NOT NULL,
                    term_id INT NOT NULL,
                    grade_id INT NOT NULL,
                    subject_id INT NOT NULL,
                    max_score DECIMAL(10,2) NOT NULL,
                    passing_score DECIMAL(10,2) NOT NULL,
                    assessment_date DATE NOT NULL,
                    created_by INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_assessments_term FOREIGN KEY (term_id) REFERENCES terms (id),
                    CONSTRAINT fk_assessments_grade FOREIGN KEY (grade_id) REFERENCES grades (id),
                    CONSTRAINT fk_assessments_subject FOREIGN KEY (subject_id) REFERENCES subjects (id)
                );

                -- Create assessment_results table
                CREATE TABLE IF NOT EXISTS assessment_results (
                    id SERIAL PRIMARY KEY,
                    assessment_id INT NOT NULL,
                    student_id INT NOT NULL,
                    score DECIMAL(10,2) NOT NULL,
                    grade VARCHAR(20) NULL,
                    remarks VARCHAR(255) NULL,
                    recorded_by INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_assessment_results_assessment FOREIGN KEY (assessment_id) REFERENCES assessments (id) ON DELETE CASCADE,
                    CONSTRAINT fk_assessment_results_student FOREIGN KEY (student_id) REFERENCES students (id) ON DELETE CASCADE
                );

                -- Create announcements table
                CREATE TABLE IF NOT EXISTS announcements (
                    id SERIAL PRIMARY KEY,
                    title VARCHAR(200) NOT NULL,
                    content TEXT NOT NULL,
                    start_date DATE NOT NULL,
                    end_date DATE NULL,
                    audience_type VARCHAR(50) NOT NULL,
                    grade_id INT NULL,
                    section_id INT NULL,
                    created_by INT NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_announcements_grade FOREIGN KEY (grade_id) REFERENCES grades (id),
                    CONSTRAINT fk_announcements_section FOREIGN KEY (section_id) REFERENCES sections (id)
                );

                -- Create messages table
                CREATE TABLE IF NOT EXISTS messages (
                    id SERIAL PRIMARY KEY,
                    sender_id INT NOT NULL,
                    recipient_id INT NOT NULL,
                    subject VARCHAR(200) NULL,
                    content TEXT NOT NULL,
                    is_read BOOLEAN NOT NULL DEFAULT FALSE,
                    read_at TIMESTAMP NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create resources table
                CREATE TABLE IF NOT EXISTS resources (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    resource_type VARCHAR(50) NOT NULL,
                    quantity INT NOT NULL DEFAULT 0,
                    available_quantity INT NOT NULL DEFAULT 0,
                    location VARCHAR(100) NULL,
                    acquisition_date DATE NULL,
                    condition VARCHAR(50) NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL
                );

                -- Create resource_allocations table
                CREATE TABLE IF NOT EXISTS resource_allocations (
                    id SERIAL PRIMARY KEY,
                    resource_id INT NOT NULL,
                    allocated_to INT NOT NULL,
                    allocation_type VARCHAR(20) NOT NULL,
                    allocation_date DATE NOT NULL,
                    return_date DATE NULL,
                    status VARCHAR(20) NOT NULL,
                    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP NULL,
                    CONSTRAINT fk_resource_allocations_resource FOREIGN KEY (resource_id) REFERENCES resources (id) ON DELETE CASCADE
                );
            ";

            await context.Database.ExecuteSqlRawAsync(sql);
        }

        /// <summary>
        /// Initializes a tenant with required data
        /// </summary>
        private async Task InitializeTenantDataAsync(ApplicationDbContext context, Tenant tenant)
        {
            // Set search path to the tenant schema
            await context.Database.ExecuteSqlRawAsync($"SET search_path TO {tenant.SchemaName}, public;");

            // Insert initial data for grades
            var gradesData = @"
                INSERT INTO grades (name, display_name, sequence, created_at)
                VALUES 
                ('KG1', 'Kindergarten 1', 1, CURRENT_TIMESTAMP),
                ('KG2', 'Kindergarten 2', 2, CURRENT_TIMESTAMP),
                ('P1', 'Primary 1', 3, CURRENT_TIMESTAMP),
                ('P2', 'Primary 2', 4, CURRENT_TIMESTAMP),
                ('P3', 'Primary 3', 5, CURRENT_TIMESTAMP),
                ('P4', 'Primary 4', 6, CURRENT_TIMESTAMP),
                ('P5', 'Primary 5', 7, CURRENT_TIMESTAMP),
                ('P6', 'Primary 6', 8, CURRENT_TIMESTAMP),
                ('JHS1', 'Junior High School 1', 9, CURRENT_TIMESTAMP),
                ('JHS2', 'Junior High School 2', 10, CURRENT_TIMESTAMP),
                ('JHS3', 'Junior High School 3', 11, CURRENT_TIMESTAMP)
                ON CONFLICT (id) DO NOTHING;
            ";

            await context.Database.ExecuteSqlRawAsync(gradesData);

            // Insert initial data for subjects
            var subjectsData = @"
                INSERT INTO subjects (name, code, description, is_active, created_at)
                VALUES 
                ('English Language', 'ENG', 'English language studies', true, CURRENT_TIMESTAMP),
                ('Mathematics', 'MATH', 'Mathematics studies', true, CURRENT_TIMESTAMP),
                ('Integrated Science', 'SCI', 'Science studies', true, CURRENT_TIMESTAMP),
                ('Social Studies', 'SOC', 'Social studies', true, CURRENT_TIMESTAMP),
                ('Religious and Moral Education', 'RME', 'Religious and moral education', true, CURRENT_TIMESTAMP),
                ('Information and Communication Technology', 'ICT', 'ICT studies', true, CURRENT_TIMESTAMP),
                ('Ghanaian Language', 'GHA', 'Local language studies', true, CURRENT_TIMESTAMP),
                ('French', 'FRE', 'French language studies', true, CURRENT_TIMESTAMP),
                ('Creative Arts', 'ARTS', 'Creative arts studies', true, CURRENT_TIMESTAMP),
                ('Physical Education', 'PE', 'Physical education', true, CURRENT_TIMESTAMP)
                ON CONFLICT (id) DO NOTHING;
            ";

            await context.Database.ExecuteSqlRawAsync(subjectsData);

            // Insert initial data for fee types
            var feeTypesData = @"
                INSERT INTO fee_types (name, description, frequency, is_mandatory, created_at)
                VALUES 
                ('Tuition Fee', 'Regular tuition fee', 'Term', true, CURRENT_TIMESTAMP),
                ('Examination Fee', 'Fee for examinations', 'Term', true, CURRENT_TIMESTAMP),
                ('Library Fee', 'Fee for library services', 'Annual', true, CURRENT_TIMESTAMP),
                ('Computer Lab Fee', 'Fee for computer lab usage', 'Annual', true, CURRENT_TIMESTAMP),
                ('Sports Fee', 'Fee for sports activities', 'Annual', true, CURRENT_TIMESTAMP),
                ('Development Levy', 'Fee for school development', 'Annual', true, CURRENT_TIMESTAMP)
                ON CONFLICT (id) DO NOTHING;
            ";

            await context.Database.ExecuteSqlRawAsync(feeTypesData);

            // Create current academic year
            var currentYear = DateTime.Now.Year;
            var academicYearData = $@"
                INSERT INTO academic_years (name, start_date, end_date, is_current, created_at)
                VALUES 
                ('{currentYear}-{currentYear + 1}', '{currentYear}-09-01', '{currentYear + 1}-07-31', true, CURRENT_TIMESTAMP)
                ON CONFLICT (id) DO NOTHING;
            ";

            await context.Database.ExecuteSqlRawAsync(academicYearData);

            // Create terms for the current academic year
            var termsData = $@"
                INSERT INTO terms (academic_year_id, name, start_date, end_date, is_current, created_at)
                VALUES 
                (1, 'First Term', '{currentYear}-09-01', '{currentYear}-12-20', true, CURRENT_TIMESTAMP),
                (1, 'Second Term', '{currentYear + 1}-01-10', '{currentYear + 1}-04-15', false, CURRENT_TIMESTAMP),
                (1, 'Third Term', '{currentYear + 1}-05-01', '{currentYear + 1}-07-31', false, CURRENT_TIMESTAMP)
                ON CONFLICT (id) DO NOTHING;
            ";

            await context.Database.ExecuteSqlRawAsync(termsData);
        }
    }
}
