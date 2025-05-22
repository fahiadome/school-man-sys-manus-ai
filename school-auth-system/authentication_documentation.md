# Authentication Feature Documentation

## Overview

This document provides comprehensive documentation for the authentication feature implemented for the multi-tenant school management system. The implementation follows clean architecture principles and uses generic repository, specific repository, unit of work, and service patterns to create a robust, maintainable, and testable authentication system.

## Architecture

The authentication feature is implemented using a layered architecture following clean architecture principles:

### 1. Domain Layer

The domain layer contains the core business entities and interfaces that are independent of any external frameworks or technologies:

- **Domain Entities**: `AuthUser`, `Role`, `Permission`, `UserRole`, `UserTenant`, `RolePermission`, `RefreshToken`
- **Repository Interfaces**: `IGenericRepository<T>`, `IAuthUserRepository`, `IRoleRepository`, `IPermissionRepository`
- **Service Interfaces**: `IAuthenticationService`, `ITokenService`, `IPasswordHasher`
- **Unit of Work Interface**: `IUnitOfWork`

### 2. Infrastructure Layer

The infrastructure layer contains implementations of the interfaces defined in the domain layer:

- **Repository Implementations**: `GenericRepository<T>`, `AuthUserRepository`, `RoleRepository`, `PermissionRepository`
- **Service Implementations**: `AuthenticationService`, `TokenService`, `PasswordHasher`
- **Unit of Work Implementation**: `UnitOfWork`
- **Database Context**: `ApplicationDbContext`

### 3. Multi-Tenancy Integration

The authentication feature is integrated with the multi-tenant architecture:

- **Tenant Context**: Authentication operations respect the current tenant context
- **Schema Selection**: Database operations use the appropriate schema based on the tenant
- **Cross-Tenant Authentication**: Support for users that can access multiple tenants

## Key Components

### Domain Entities

#### AuthUser

The `AuthUser` entity represents a user in the authentication system:

- Core properties: `Id`, `Username`, `Email`, `PasswordHash`, `Salt`
- Security properties: `EmailConfirmed`, `IsActive`, `FailedLoginAttempts`, `LockoutEnd`
- Audit properties: `LastLogin`, `CreatedAt`, `UpdatedAt`
- Navigation properties: `UserRoles`, `UserTenants`, `RefreshTokens`
- Domain methods: `ConfirmEmail()`, `UpdatePassword()`, `RecordSuccessfulLogin()`, `RecordFailedLogin()`, `IsLockedOut()`, `Activate()`, `Deactivate()`, `AddRefreshToken()`, `RevokeRefreshToken()`

#### Role

The `Role` entity represents a role in the authentication system:

- Core properties: `Id`, `Name`, `Description`, `IsSystemRole`
- Audit properties: `CreatedAt`, `UpdatedAt`
- Navigation properties: `UserRoles`, `RolePermissions`
- Domain methods: `Update()`, `AddPermission()`, `RemovePermission()`

#### Permission

The `Permission` entity represents a permission in the authentication system:

- Core properties: `Id`, `Name`, `Description`, `Resource`
- Audit properties: `CreatedAt`, `UpdatedAt`
- Domain methods: `Update()`

#### RefreshToken

The `RefreshToken` entity represents a refresh token for JWT authentication:

- Core properties: `Id`, `Token`, `ExpiryDate`, `IsExpired`, `IsActive`
- Audit properties: `CreatedAt`, `CreatedByIp`, `RevokedAt`, `RevokedByIp`, `ReplacedByToken`
- Navigation properties: `UserId`, `User`
- Domain methods: `Revoke()`

### Repository Pattern

#### Generic Repository

The `GenericRepository<T>` provides a standard interface for data access operations:

- CRUD operations: `GetByIdAsync()`, `GetAllAsync()`, `FindAsync()`, `AddAsync()`, `UpdateAsync()`, `DeleteAsync()`, `DeleteByIdAsync()`
- Query operations: `ExistsAsync()`, `CountAsync()`

#### Specific Repositories

Specific repositories extend the generic repository with domain-specific operations:

- `AuthUserRepository`: User-specific operations like `GetByUsernameAsync()`, `GetByEmailAsync()`, `GetWithRolesAsync()`
- `RoleRepository`: Role-specific operations like `GetByNameAsync()`, `GetWithPermissionsAsync()`, `GetRolesByUserAsync()`
- `PermissionRepository`: Permission-specific operations like `GetByNameAsync()`, `GetByResourceAsync()`, `GetPermissionsByRoleAsync()`

### Unit of Work Pattern

The `UnitOfWork` coordinates the work of multiple repositories:

- Repository access: `AuthUsers`, `Roles`, `Permissions`
- Transaction management: `BeginTransactionAsync()`, `CommitTransactionAsync()`, `RollbackTransactionAsync()`
- Persistence: `SaveChangesAsync()`

### Service Layer

#### Authentication Service

The `AuthenticationService` orchestrates the authentication process:

- User management: `RegisterAsync()`, `ChangePasswordAsync()`, `ForgotPasswordAsync()`, `ResetPasswordAsync()`
- Authentication: `AuthenticateAsync()`, `RefreshTokenAsync()`, `RevokeTokenAsync()`, `ValidateTokenAsync()`

#### Token Service

The `TokenService` handles JWT token generation and validation:

- Token operations: `GenerateJwtToken()`, `ValidateToken()`, `GenerateRefreshToken()`

#### Password Hasher

The `PasswordHasher` handles secure password hashing and verification:

- Password operations: `HashPassword()`, `VerifyPassword()`

### Database Context

The `ApplicationDbContext` provides access to the database with multi-tenant support:

- Entity configuration: `OnModelCreating()`
- Schema configuration: `ConfigureCommonSchema()`, `ConfigureMasterSchema()`, `ConfigureTenantSchema()`
- Tenant context: Sets the appropriate schema search path based on the current tenant

## Multi-Tenancy Strategy

The authentication feature supports a multi-tenant architecture with a shared database and separate schemas:

1. **Common Schema**: Contains shared authentication entities (`users`, `roles`, `permissions`, etc.)
2. **Master Schema**: Contains tenant management entities (`tenants`)
3. **Tenant-Specific Schemas**: Contains tenant-specific entities (not used for authentication in this implementation)

The `ApplicationDbContext` dynamically selects the appropriate schema based on the current tenant context, ensuring that authentication operations respect tenant boundaries.

## Authentication Flow

### Registration

1. User provides username, email, and password
2. System validates inputs and checks for existing username/email
3. Password is hashed with a secure algorithm
4. User entity is created and saved to the database
5. If a tenant ID is provided, the user is associated with the tenant

### Authentication

1. User provides username/email and password
2. System validates inputs and finds the user
3. System checks if the user is active and not locked out
4. Password is verified against the stored hash
5. If successful, the system records the login and generates JWT and refresh tokens
6. System returns authentication result with tokens, roles, permissions, and tenant information

### Token Refresh

1. User provides a refresh token
2. System validates the token and finds the associated user
3. System revokes the old token and generates new JWT and refresh tokens
4. System returns updated authentication result

### Password Management

1. **Change Password**: User provides current and new passwords, system verifies current password and updates to new password
2. **Forgot Password**: User provides email, system generates a reset token
3. **Reset Password**: User provides email, reset token, and new password, system verifies token and updates password

## Security Considerations

### Password Security

- Passwords are hashed using PBKDF2 with HMAC-SHA256
- A unique salt is generated for each password
- 10,000 iterations are used for key derivation

### Token Security

- JWT tokens are signed with a secure secret key
- Tokens have a configurable expiration time
- Refresh tokens are securely stored in the database
- Refresh tokens can be revoked

### Brute Force Protection

- Failed login attempts are tracked
- Accounts are locked after a configurable number of failed attempts
- Lockout duration is configurable

## Usage Examples

### Registering a User

```csharp
// Inject IAuthenticationService
private readonly IAuthenticationService _authService;

// Register a new user
var user = await _authService.RegisterAsync(
    username: "john.doe",
    email: "john.doe@example.com",
    password: "SecurePassword123!",
    tenantId: 1);
```

### Authenticating a User

```csharp
// Authenticate a user
var result = await _authService.AuthenticateAsync(
    usernameOrEmail: "john.doe@example.com",
    password: "SecurePassword123!",
    ipAddress: "192.168.1.1");

// Use the authentication result
var token = result.Token;
var refreshToken = result.RefreshToken;
var roles = result.Roles;
var permissions = result.Permissions;
```

### Refreshing a Token

```csharp
// Refresh a token
var result = await _authService.RefreshTokenAsync(
    refreshToken: "refresh_token_value",
    ipAddress: "192.168.1.1");

// Use the updated authentication result
var newToken = result.Token;
var newRefreshToken = result.RefreshToken;
```

### Changing a Password

```csharp
// Change a password
var success = await _authService.ChangePasswordAsync(
    userId: 1,
    currentPassword: "CurrentPassword123!",
    newPassword: "NewPassword456!");
```

## Validation Against Requirements

### Core Authentication Requirements

| Requirement | Implementation | Status |
|-------------|----------------|--------|
| User Registration | `AuthenticationService.RegisterAsync()` | ✅ Complete |
| User Login | `AuthenticationService.AuthenticateAsync()` | ✅ Complete |
| Token-based Authentication | JWT token generation and validation | ✅ Complete |
| Password Management | Secure hashing, verification, reset | ✅ Complete |
| Multi-tenant Context | Tenant-aware authentication | ✅ Complete |
| Role-based Authorization | User roles and permissions | ✅ Complete |
| Account Management | Password reset, account locking | ✅ Complete |

### Multi-tenancy Considerations

| Requirement | Implementation | Status |
|-------------|----------------|--------|
| Tenant-specific Authentication | Tenant context in authentication | ✅ Complete |
| Cross-tenant Authentication | Support for users in multiple tenants | ✅ Complete |
| Tenant Resolution | Integration with tenant resolution middleware | ✅ Complete |
| Tenant-specific Roles | Role assignment within tenant context | ✅ Complete |

### Security Requirements

| Requirement | Implementation | Status |
|-------------|----------------|--------|
| Password Security | PBKDF2 with HMAC-SHA256, unique salt | ✅ Complete |
| Token Security | Signed JWT tokens with expiration | ✅ Complete |
| Brute Force Protection | Account lockout after failed attempts | ✅ Complete |
| Audit Trail | Login tracking and token management | ✅ Complete |

## Clean Architecture Validation

| Principle | Implementation | Status |
|-----------|----------------|--------|
| Dependency Rule | Dependencies point inward to domain | ✅ Complete |
| Separation of Concerns | Each layer has specific responsibility | ✅ Complete |
| Dependency Inversion | High-level modules don't depend on low-level modules | ✅ Complete |
| Interface Segregation | Clients don't depend on methods they don't use | ✅ Complete |
| Single Responsibility | Each class has one reason to change | ✅ Complete |

## Conclusion

The authentication feature for the multi-tenant school management system has been implemented following clean architecture principles and using generic repository, specific repository, unit of work, and service patterns. The implementation provides a robust, maintainable, and testable authentication system that supports multi-tenancy and follows security best practices.

The feature is ready for integration with the rest of the application and can be extended with additional functionality as needed.
