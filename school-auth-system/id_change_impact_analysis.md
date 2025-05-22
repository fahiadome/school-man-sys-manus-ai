# Impact Analysis: Changing Entity IDs to String GUIDs

## Overview
This document analyzes the impact of changing all entity ID properties from integer types to string GUIDs across the authentication system.

## Affected Components

### 1. Domain Entities
All domain entities need their ID properties changed from `int` to `string` with GUID initialization:
- AuthUser
- Role
- Permission
- UserRole
- UserTenant
- RolePermission
- RefreshToken
- Tenant

### 2. Entity Relationships
All foreign key relationships need to be updated to use string IDs:
- One-to-many relationships (e.g., User to RefreshTokens)
- Many-to-many relationships (e.g., UserRoles, RolePermissions)
- Composite keys in join entities

### 3. Repository Interfaces
Repository interfaces need parameter type changes:
- IGenericRepository<T>: GetByIdAsync(int id) → GetByIdAsync(string id)
- All specific repository methods with ID parameters

### 4. Repository Implementations
All repository implementations need to be updated:
- Parameter types in method signatures
- Query expressions using ID properties

### 5. Unit of Work
The Unit of Work pattern may need updates if it directly references entity IDs.

### 6. Service Layer
Authentication and other services need updates:
- Method signatures with ID parameters
- Business logic that manipulates or compares IDs

### 7. Database Context
The ApplicationDbContext needs updates:
- Entity configurations
- Key definitions
- Foreign key relationships

### 8. Potential Breaking Changes
- API signatures will change (int → string)
- Existing data migrations may be affected
- Any code that assumes integer IDs will need revision

## Implementation Strategy

1. Update all domain entity ID properties
2. Update all relationship configurations
3. Update repository interfaces and implementations
4. Update service layer components
5. Update database context configurations
6. Validate all changes for consistency

## Considerations

### Performance
- String GUIDs as primary keys may have performance implications compared to integers
- Index considerations for string-based primary keys

### Storage
- String GUIDs require more storage space than integers

### Security
- GUIDs provide better security by being non-sequential and harder to guess

### Distributed Systems
- GUIDs enable ID generation without central coordination, beneficial for distributed systems

## Conclusion
Changing from integer IDs to string GUIDs is a significant architectural change that affects multiple layers of the application. The implementation requires careful coordination to ensure all components are updated consistently while maintaining the clean architecture principles.
