# ID Property Change Documentation

## Overview
This document details the changes made to convert all entity ID properties from integer types to string GUIDs across the multi-tenant school management system. This change improves the system's scalability, reduces the risk of ID collisions, and enhances security by making IDs non-sequential.

## Scope of Changes

### Entity Models
- Updated the base `BaseEntity` class to use string GUID IDs with automatic generation
- Modified all entity classes that inherit from `BaseEntity` to use the new ID type
- Updated entities with composite keys and direct ID definitions to use string GUIDs
- Changed all foreign key properties to string type to maintain referential integrity

### Database Configuration
- Updated `ApplicationDbContext` configurations to support string GUID IDs
- Modified entity relationship configurations to maintain proper foreign key constraints
- Ensured all composite key definitions use string type

### Authentication Domain
- Verified and updated authentication domain entities to use string GUIDs
- Ensured consistency between main domain and authentication domain entities
- Updated cross-domain references to maintain type compatibility

### Repository and Service Layers
- Modified `IGenericRepository<T>` interface to use string IDs in all methods
- Updated all repository implementations to handle string GUID IDs
- Ensured service layer implementations properly handle string IDs
- Verified all query methods use the correct ID type

## Implementation Details

### ID Generation Strategy
All entities now use the following ID generation approach:
```csharp
public string Id { get; set; } = Guid.NewGuid().ToString("N");
```

This generates a 32-character string GUID without hyphens, providing:
- Globally unique identifiers
- No sequential pattern that could be exploited
- Consistent format across all entities

### Foreign Key Relationships
All foreign key properties have been updated to use string type:
```csharp
// Before
public int StudentId { get; set; }

// After
public string StudentId { get; set; }
```

### Composite Keys
Entities with composite keys have been updated to use string type for all key parts:
```csharp
// Before
[Key, Column(Order = 0)]
public int UserId { get; set; }
        
[Key, Column(Order = 1)]
public int RoleId { get; set; }

// After
[Key, Column(Order = 0)]
public string UserId { get; set; }
        
[Key, Column(Order = 1)]
public string RoleId { get; set; }
```

## Migration Considerations
When deploying these changes to an existing database:

1. A database migration will be required to convert all ID columns from integer to string
2. Data migration will need to convert existing integer IDs to string GUIDs
3. All foreign key constraints will need to be dropped and recreated
4. Application downtime will be necessary during the migration

## Benefits

1. **Scalability**: GUIDs can be generated without a central authority, making distributed systems easier to implement
2. **Security**: Non-sequential IDs prevent enumeration attacks
3. **Uniqueness**: Virtually eliminates the risk of ID collisions across tenants
4. **Consistency**: Provides a uniform ID strategy across all entities

## Potential Considerations

1. **Storage**: String GUIDs require more storage space than integers
2. **Performance**: String comparison is slightly slower than integer comparison
3. **Readability**: GUIDs are less human-readable than sequential integers

## Conclusion
The conversion to string GUID IDs provides significant benefits for the multi-tenant school management system, particularly in terms of scalability and security. The implementation has been thoroughly validated to ensure consistency across all layers of the application.
