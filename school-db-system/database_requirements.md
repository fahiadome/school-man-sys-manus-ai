# Multi-Tenant School Management System Database Requirements

Based on the PRD for the Ghanaian Basic and JHS School Management System, the following database requirements have been identified for implementing a multi-tenant architecture using a shared database with different schemas approach in ASP.NET Core 6 and PostgreSQL.

## Multi-Tenancy Strategy

The system will use a **shared database with separate schemas** approach where:
- Each tenant (school) will have its own schema within a single PostgreSQL database
- Common/shared data will be stored in a separate schema accessible to all tenants
- Tenant isolation will be enforced at the application level through schema-based separation

## Core Entities

### Tenant Management
- **Tenant**: Represents a school with basic information and configuration
- **TenantSettings**: Tenant-specific configurations and customizations
- **Subscription**: Subscription and billing information for each tenant

### User Management
- **User**: Core user information shared across all tenants
- **Role**: User roles (System Admin, School Admin, Teacher, Student, Parent, etc.)
- **Permission**: Granular permissions for role-based access control
- **UserRole**: Many-to-many relationship between users and roles
- **UserTenant**: Associates users with tenants they have access to

### Student Information
- **Student**: Student personal and demographic information
- **Guardian**: Parent/guardian information
- **StudentGuardian**: Relationship between students and guardians
- **Enrollment**: Student enrollment records for academic terms
- **Attendance**: Daily and subject-wise attendance records
- **HealthRecord**: Student health information and medical conditions
- **Behavior**: Behavioral records and disciplinary history

### Academic Management
- **AcademicYear**: Academic year definition
- **Term**: Term/semester within an academic year
- **Grade**: Grade/class level (e.g., Primary 1, JHS 3)
- **Section**: Divisions within a grade (e.g., Class 1A, 1B)
- **Subject**: Academic subjects
- **Curriculum**: Curriculum definition and mapping
- **Timetable**: Class scheduling information
- **Lesson**: Lesson planning and resources
- **Assignment**: Homework and assignments
- **Assessment**: Tests, exams, and other assessments
- **Grade**: Student grades and performance records

### Staff Management
- **Staff**: Staff personal and professional information
- **StaffType**: Categories of staff (teaching, administrative, support)
- **Qualification**: Staff qualifications and certifications
- **TeacherSubject**: Subjects taught by teachers
- **TeacherClass**: Classes assigned to teachers
- **StaffAttendance**: Staff attendance records
- **Leave**: Staff leave requests and approvals
- **Performance**: Staff performance evaluations

### Financial Management
- **FeeType**: Types of fees (tuition, examination, etc.)
- **FeeStructure**: Fee amounts for different grades/terms
- **Invoice**: Student fee invoices
- **Payment**: Fee payments and receipts
- **Expense**: School expenses and expenditures
- **Budget**: Budget planning and allocation
- **Salary**: Staff salary information

### Resource Management
- **Resource**: Educational resources and materials
- **ResourceType**: Categories of resources
- **Inventory**: Inventory tracking for resources
- **Facility**: School facilities and infrastructure
- **Maintenance**: Facility maintenance records
- **Library**: Library catalog and circulation

### Communication
- **Announcement**: School-wide and class announcements
- **Message**: Direct messages between users
- **Notification**: System notifications and alerts
- **Event**: School events and activities calendar

## Database Schema Design Considerations

1. **Tenant Isolation**:
   - Each tenant's data will be isolated in its own schema
   - Schema names will follow a pattern like `tenant_{tenant_id}`
   - A master schema will contain tenant registration information

2. **Cross-Tenant Data**:
   - Shared reference data will be stored in a common schema
   - Users can belong to multiple tenants with different roles

3. **Performance Optimization**:
   - Appropriate indexes for frequently queried fields
   - Partitioning strategies for large tables
   - Efficient query patterns for multi-tenant data access

4. **Localization Support**:
   - Support for multiple languages including Ghanaian languages
   - Culturally appropriate data formats and conventions

5. **Offline Capability**:
   - Data structures to support synchronization
   - Conflict resolution mechanisms
   - Change tracking for offline operations

6. **Audit and Compliance**:
   - Comprehensive audit logging
   - Data retention policies
   - GDPR and Ghana Data Protection Act compliance

7. **Scalability**:
   - Design to support up to 5,000 schools (tenants)
   - Support for up to 2 million students
   - Support for up to 200,000 teachers and staff
