# Multi-Tenant Database Schema Design

This document outlines the database schema design for the multi-tenant school management system using a shared database with different schemas approach in PostgreSQL.

## Schema Structure

The database will be organized into the following schemas:

1. **public** - Contains system-wide tables and functions
2. **master** - Contains tenant registration and management tables
3. **common** - Contains shared reference data accessible to all tenants
4. **tenant_{id}** - Tenant-specific schemas (one per school)

## Schema Diagrams

### Master Schema

```
master.tenants
├── id (PK)
├── name
├── schema_name
├── domain
├── connection_string
├── is_active
├── created_at
├── updated_at
└── settings (JSONB)

master.tenant_subscriptions
├── id (PK)
├── tenant_id (FK)
├── plan_id
├── start_date
├── end_date
├── is_trial
├── status
└── payment_status
```

### Common Schema

```
common.users
├── id (PK)
├── username
├── email
├── password_hash
├── first_name
├── last_name
├── phone_number
├── is_active
├── last_login
├── created_at
└── updated_at

common.roles
├── id (PK)
├── name
├── description
└── is_system_role

common.permissions
├── id (PK)
├── name
├── description
└── resource

common.role_permissions
├── role_id (PK, FK)
├── permission_id (PK, FK)
└── created_at

common.user_tenants
├── user_id (PK, FK)
├── tenant_id (PK, FK)
├── is_primary
└── created_at

common.user_roles
├── user_id (PK, FK)
├── role_id (PK, FK)
├── tenant_id (FK)
└── created_at
```

### Tenant-Specific Schema (tenant_{id})

```
students
├── id (PK)
├── user_id (FK)
├── admission_number
├── date_of_birth
├── gender
├── address
├── nationality
├── religion
├── admission_date
├── current_grade_id (FK)
├── current_section_id (FK)
├── status
├── photo_url
├── created_at
└── updated_at

guardians
├── id (PK)
├── user_id (FK)
├── relationship
├── occupation
├── work_address
├── education_level
├── created_at
└── updated_at

student_guardians
├── student_id (PK, FK)
├── guardian_id (PK, FK)
├── is_primary
├── can_pickup
├── emergency_contact
└── created_at

academic_years
├── id (PK)
├── name
├── start_date
├── end_date
├── is_current
├── created_at
└── updated_at

terms
├── id (PK)
├── academic_year_id (FK)
├── name
├── start_date
├── end_date
├── is_current
├── created_at
└── updated_at

grades
├── id (PK)
├── name
├── display_name
├── sequence
├── created_at
└── updated_at

sections
├── id (PK)
├── grade_id (FK)
├── name
├── capacity
├── room
├── created_at
└── updated_at

subjects
├── id (PK)
├── name
├── code
├── description
├── is_active
├── created_at
└── updated_at

enrollments
├── id (PK)
├── student_id (FK)
├── academic_year_id (FK)
├── term_id (FK)
├── grade_id (FK)
├── section_id (FK)
├── enrollment_date
├── status
├── created_at
└── updated_at

attendance
├── id (PK)
├── student_id (FK)
├── date
├── status
├── reason
├── recorded_by
├── created_at
└── updated_at

subject_attendance
├── id (PK)
├── student_id (FK)
├── subject_id (FK)
├── date
├── status
├── reason
├── recorded_by
├── created_at
└── updated_at

staff
├── id (PK)
├── user_id (FK)
├── employee_id
├── date_of_birth
├── gender
├── address
├── joining_date
├── designation
├── department
├── qualification
├── experience
├── status
├── photo_url
├── created_at
└── updated_at

teacher_subjects
├── teacher_id (PK, FK)
├── subject_id (PK, FK)
├── academic_year_id (FK)
├── created_at
└── updated_at

teacher_sections
├── teacher_id (PK, FK)
├── section_id (PK, FK)
├── academic_year_id (FK)
├── is_class_teacher
├── created_at
└── updated_at

staff_attendance
├── id (PK)
├── staff_id (FK)
├── date
├── status
├── reason
├── recorded_by
├── created_at
└── updated_at

assessments
├── id (PK)
├── name
├── description
├── assessment_type
├── term_id (FK)
├── grade_id (FK)
├── subject_id (FK)
├── max_score
├── passing_score
├── assessment_date
├── created_by
├── created_at
└── updated_at

assessment_results
├── id (PK)
├── assessment_id (FK)
├── student_id (FK)
├── score
├── grade
├── remarks
├── recorded_by
├── created_at
└── updated_at

fee_types
├── id (PK)
├── name
├── description
├── frequency
├── is_mandatory
├── created_at
└── updated_at

fee_structures
├── id (PK)
├── fee_type_id (FK)
├── grade_id (FK)
├── academic_year_id (FK)
├── term_id (FK)
├── amount
├── due_date
├── created_at
└── updated_at

invoices
├── id (PK)
├── student_id (FK)
├── academic_year_id (FK)
├── term_id (FK)
├── invoice_number
├── issue_date
├── due_date
├── total_amount
├── discount
├── net_amount
├── status
├── created_at
└── updated_at

invoice_items
├── id (PK)
├── invoice_id (FK)
├── fee_structure_id (FK)
├── amount
├── discount
├── net_amount
├── created_at
└── updated_at

payments
├── id (PK)
├── invoice_id (FK)
├── payment_date
├── amount
├── payment_method
├── reference_number
├── received_by
├── remarks
├── created_at
└── updated_at

announcements
├── id (PK)
├── title
├── content
├── start_date
├── end_date
├── audience_type
├── grade_id (FK, nullable)
├── section_id (FK, nullable)
├── created_by
├── created_at
└── updated_at

messages
├── id (PK)
├── sender_id (FK)
├── recipient_id (FK)
├── subject
├── content
├── is_read
├── read_at
├── created_at
└── updated_at

resources
├── id (PK)
├── name
├── resource_type
├── quantity
├── available_quantity
├── location
├── acquisition_date
├── condition
├── created_at
└── updated_at

resource_allocations
├── id (PK)
├── resource_id (FK)
├── allocated_to
├── allocation_type
├── allocation_date
├── return_date
├── status
├── created_at
└── updated_at
```

## Multi-Tenancy Implementation

### Tenant Resolution

The system will use a combination of hostname and explicit tenant selection for tenant resolution:

1. **Hostname-based resolution**: Each tenant can have a custom domain that maps to their tenant ID
2. **Explicit selection**: Users with access to multiple tenants can explicitly select which tenant to access

### Connection Management

The application will use a single connection string to the PostgreSQL database but will set the appropriate schema search path for each request based on the resolved tenant:

```sql
SET search_path TO tenant_{id}, common, public;
```

### Entity Framework Core Configuration

The DbContext will be configured to use the appropriate schema for each entity:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Common schema entities
    modelBuilder.Entity<User>().ToTable("users", "common");
    modelBuilder.Entity<Role>().ToTable("roles", "common");
    
    // Tenant-specific entities will use the schema determined at runtime
    modelBuilder.Entity<Student>().ToTable("students", _tenantInfo.SchemaName);
    modelBuilder.Entity<Guardian>().ToTable("guardians", _tenantInfo.SchemaName);
    
    // Additional configuration...
}
```

### Tenant Middleware

A middleware component will be responsible for:

1. Resolving the current tenant based on the request
2. Setting up the appropriate DbContext with the correct schema
3. Ensuring tenant isolation throughout the request lifecycle

## Data Access Patterns

### Repository Pattern

The system will implement the repository pattern to abstract data access logic:

```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### Unit of Work

A unit of work pattern will be used to manage transactions across multiple repositories:

```csharp
public interface IUnitOfWork
{
    IStudentRepository Students { get; }
    IGuardianRepository Guardians { get; }
    // Other repositories...
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

## Migration Strategy

The code-first approach will generate migrations for:

1. The master schema (tenant management)
2. The common schema (shared data)
3. A template for tenant-specific schemas

When a new tenant is provisioned, the system will:

1. Create a new schema for the tenant
2. Apply the tenant template migrations to the new schema
3. Initialize the schema with required reference data

## Indexes and Performance Optimization

Key indexes will be created for:

1. Foreign keys for relationship navigation
2. Frequently queried fields (e.g., email, admission_number)
3. Date fields used for filtering (e.g., created_at, attendance_date)
4. Fields used in reporting and analytics

## Audit and Data Tracking

The system will implement:

1. Created/updated timestamps on all entities
2. User tracking for create/update operations
3. Separate audit tables for sensitive operations
4. Change tracking for offline synchronization
