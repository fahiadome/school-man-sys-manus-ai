# Multi-Tenant School Management System - Schema and Multi-Tenancy Strategy Documentation

## Overview

This document provides a comprehensive overview of the database schema and multi-tenancy strategy implemented for the Ghanaian Basic and JHS School Management System. The system uses ASP.NET Core 6 with PostgreSQL and follows a code-first approach to implement a shared database with different schema multi-tenancy model.

## Multi-Tenancy Strategy

### Approach: Shared Database with Separate Schemas

The system implements a multi-tenant architecture using a shared database with separate schemas approach, where:

1. **Single Database**: All tenants (schools) share a single PostgreSQL database instance
2. **Schema Separation**: Each tenant has its own dedicated schema within the database
3. **Schema Naming Convention**: Tenant schemas follow the naming pattern `tenant_{id}`
4. **Shared Data**: Common reference data is stored in a separate `common` schema
5. **Tenant Management**: Tenant registration and management data is stored in a `master` schema

### Benefits of This Approach

1. **Data Isolation**: Each tenant's data is logically separated at the database level
2. **Resource Efficiency**: Database resources are shared across tenants
3. **Simplified Maintenance**: Single database instance to maintain and backup
4. **Cost Effectiveness**: Lower infrastructure costs compared to database-per-tenant
5. **Scalability**: Can support thousands of tenants on a single database instance
6. **Customization**: Each tenant can have schema variations without affecting others

### Tenant Resolution and Context

The system resolves the current tenant through:

1. **Domain-based Resolution**: Each tenant can have a custom domain
2. **Header-based Resolution**: API calls can specify tenant via `X-Tenant-ID` header
3. **Middleware Processing**: `TenantResolutionMiddleware` resolves tenant for each request
4. **Context Scoping**: Tenant context is maintained throughout the request lifecycle

## Schema Structure

The database is organized into the following schemas:

### 1. Master Schema (`master`)

Contains tenant registration and management tables:

- `tenants`: Core tenant information and configuration
- `tenant_subscriptions`: Subscription and billing information

### 2. Common Schema (`common`)

Contains shared reference data accessible to all tenants:

- `users`: Core user information shared across tenants
- `roles`: System and tenant roles
- `permissions`: Granular permissions for RBAC
- `user_roles`: User-role associations
- `user_tenants`: User-tenant associations
- `role_permissions`: Role-permission associations

### 3. Tenant-Specific Schemas (`tenant_{id}`)

Each tenant has its own schema containing:

#### Academic Management
- `academic_years`: Academic year definitions
- `terms`: Terms/semesters within academic years
- `grades`: Grade/class levels
- `sections`: Divisions within grades
- `subjects`: Academic subjects

#### Student Management
- `students`: Student information
- `guardians`: Parent/guardian information
- `student_guardians`: Student-guardian relationships
- `enrollments`: Student enrollment records
- `attendance`: Attendance records

#### Staff Management
- `staff`: Staff information
- `teacher_subjects`: Subject assignments
- `teacher_sections`: Class assignments

#### Financial Management
- `fee_types`: Types of fees
- `fee_structures`: Fee amounts by grade/term
- `invoices`: Student fee invoices
- `invoice_items`: Line items in invoices
- `payments`: Fee payment records

#### Assessment Management
- `assessments`: Test and exam definitions
- `assessment_results`: Student assessment results

#### Communication
- `announcements`: School announcements
- `messages`: Direct messages

#### Resource Management
- `resources`: Educational resources
- `resource_allocations`: Resource assignments

## Entity Relationships

The database schema implements the following key relationships:

1. **User-Tenant Relationships**:
   - Users can belong to multiple tenants with different roles
   - Each user-tenant association is tracked in `common.user_tenants`
   - Role assignments within tenants are tracked in `common.user_roles`

2. **Academic Structure**:
   - Academic years contain terms
   - Grades contain sections
   - Students are enrolled in specific grade/section combinations

3. **Student Relationships**:
   - Students are linked to user accounts
   - Students can have multiple guardians
   - Students have enrollments, attendance, and assessment results

4. **Staff Relationships**:
   - Staff members are linked to user accounts
   - Teachers can be assigned to multiple subjects and sections

5. **Financial Relationships**:
   - Fee structures are defined per grade/term
   - Invoices contain multiple invoice items
   - Payments are linked to invoices

## Code-First Implementation

### Entity Framework Core Configuration

The system uses Entity Framework Core with a code-first approach:

1. **Dynamic Schema Selection**:
   - `ApplicationDbContext` configures entity schemas at runtime
   - Tenant-specific entities use the current tenant's schema
   - Common entities always use the `common` schema
   - Master entities always use the `master` schema

2. **Entity Configuration**:
   - Entities are configured using Fluent API in `OnModelCreating`
   - Schema names are applied using `.ToTable("table_name", schema_name)`
   - Relationships are configured with appropriate cascade behaviors

3. **Tenant Context Injection**:
   - `ITenantInfo` is injected into `ApplicationDbContext`
   - Schema selection is based on the current tenant context

### Migration Strategy

The system implements a two-phase migration approach:

1. **Base Schema Migration**:
   - Creates and migrates `master` and `common` schemas
   - Executed at application startup

2. **Tenant Schema Provisioning**:
   - Creates tenant-specific schema when a new tenant is registered
   - Applies tenant template migrations to the new schema
   - Initializes the schema with required reference data

### Tenant Provisioning Process

When a new tenant is provisioned:

1. A new record is created in `master.tenants`
2. A new schema is created with the pattern `tenant_{id}`
3. Tenant template migrations are applied to create all required tables
4. Initial reference data is seeded (grades, subjects, fee types, etc.)
5. The tenant is now ready for use

## Schema Evolution and Upgrades

The system supports schema evolution through:

1. **Versioned Migrations**:
   - New migrations are created for schema changes
   - Migrations are applied to all tenant schemas

2. **Schema Compatibility**:
   - Backward compatibility is maintained for existing tenants
   - New tenants get the latest schema version

3. **Migration Orchestration**:
   - `TenantMigrationService` coordinates migrations across schemas
   - Ensures consistency across all tenant schemas

## Performance Considerations

The schema design incorporates several performance optimizations:

1. **Indexing Strategy**:
   - Primary keys are automatically indexed
   - Foreign keys are indexed for relationship navigation
   - Additional indexes on frequently queried fields

2. **Query Optimization**:
   - Schema search path is set for each request
   - Tenant isolation at the schema level reduces query complexity

3. **Connection Pooling**:
   - Single connection string for all tenants
   - Connection pool is shared across tenants

## Security and Data Isolation

The multi-tenant architecture ensures data isolation through:

1. **Schema-Level Isolation**:
   - PostgreSQL schema permissions prevent cross-tenant access
   - Each request operates within a specific schema context

2. **Application-Level Security**:
   - Tenant resolution middleware validates tenant access
   - Role-based access control within each tenant
   - Permission checks at the API level

3. **Audit Trail**:
   - All entities track creation and modification timestamps
   - User tracking for sensitive operations

## Offline Capability and Synchronization

The schema supports offline operations through:

1. **Change Tracking**:
   - Updated timestamps on all entities
   - Version numbers for conflict resolution

2. **Batch Synchronization**:
   - APIs for bulk data operations
   - Conflict resolution mechanisms

## Conclusion

The multi-tenant database schema for the Ghanaian Basic and JHS School Management System provides a robust, scalable, and secure foundation for managing multiple schools within a single database instance. The schema-per-tenant approach offers an optimal balance between resource efficiency and tenant isolation, while the code-first implementation ensures maintainability and extensibility as the system evolves.
