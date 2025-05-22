# Akka.NET Actor Model Audit Logging Architecture

This document provides a detailed overview of the architecture for the Akka.NET actor-based audit logging solution implemented for the multi-tenant school management system.

## Actor Hierarchy

The solution implements a hierarchical actor system with specialized actors for different responsibilities:

```
AuditCoordinatorActor
├── TenantManagerActor
│   ├── TenantActor (Tenant 1)
│   │   ├── EntityTypeActor (Student)
│   │   ├── EntityTypeActor (Grade)
│   │   └── EntityTypeActor (...)
│   ├── TenantActor (Tenant 2)
│   │   └── ...
│   └── ...
├── PersistenceManagerActor
│   ├── BatchProcessorActor (Tenant 1)
│   ├── BatchProcessorActor (Tenant 2)
│   └── ...
└── AuditQueryActor
    ├── TenantQueryActor (Tenant 1)
    ├── TenantQueryActor (Tenant 2)
    └── ...
```

## Actor Responsibilities

### AuditCoordinatorActor
- Entry point for all audit operations
- Coordinates message routing to child actors
- Implements supervision strategy for child actors
- Handles actor lifecycle management

### TenantManagerActor
- Manages tenant-specific audit actors
- Creates tenant actors on demand
- Routes audit commands to appropriate tenant actors
- Implements tenant-level supervision strategy

### TenantActor
- Handles audit events for a specific tenant
- Implements tenant-specific throttling
- Creates and manages entity type actors
- Ensures tenant isolation

### EntityTypeActor
- Processes audit events for specific entity types
- Buffers events for batch processing
- Schedules periodic batch flushing
- Optimizes for entity-specific processing

### PersistenceManagerActor
- Coordinates database interactions
- Creates and manages batch processor actors
- Implements database-level supervision strategy
- Handles persistence failures

### BatchProcessorActor
- Batches audit events for efficient storage
- Implements persistent actor pattern for reliability
- Manages transaction boundaries
- Optimizes database operations

### AuditQueryActor
- Handles audit trail retrieval requests
- Routes queries to appropriate tenant query actors
- Implements query-specific supervision strategy
- Manages query result aggregation

## Message Flow

1. **Create/Update/Delete Operation**:
   - Application calls `AuditService.LogXxxAsync()`
   - `AuditService` creates appropriate command
   - Command sent to `AuditCoordinatorActor`

2. **Command Processing**:
   - `AuditCoordinatorActor` routes to `TenantManagerActor`
   - `TenantManagerActor` routes to specific `TenantActor`
   - `TenantActor` applies throttling and routes to `EntityTypeActor`
   - `EntityTypeActor` buffers event and periodically flushes to `PersistenceManagerActor`
   - `PersistenceManagerActor` routes to `BatchProcessorActor`
   - `BatchProcessorActor` persists batch to database

3. **Query Processing**:
   - Application calls `AuditService.GetAuditTrailAsync()`
   - `AuditService` creates query
   - Query sent to `AuditQueryActor`
   - `AuditQueryActor` routes to specific `TenantQueryActor`
   - `TenantQueryActor` retrieves data from database
   - Results returned to caller

## Supervision Strategies

Each level of the actor hierarchy implements appropriate supervision strategies:

- **AuditCoordinatorActor**: Restarts child actors on initialization errors, stops on kill requests
- **TenantManagerActor**: Restarts on database errors, resumes on other exceptions
- **TenantActor**: Restarts entity type actors on errors
- **PersistenceManagerActor**: Restarts on database and timeout errors

## Throttling Mechanism

The `TenantActor` implements a token bucket algorithm for throttling:

- Each tenant has a maximum events per second limit
- Burst capacity allows for temporary spikes
- Events beyond limits are rejected with throttling exceptions
- Throttling is tenant-specific to prevent noisy neighbor problems

## Batching Strategy

The `EntityTypeActor` implements event batching:

- Events are buffered in memory
- Batches are flushed when:
  - Batch size threshold is reached
  - Scheduled flush interval elapses
  - Actor is stopping

## Persistence Strategy

The `BatchProcessorActor` implements reliable persistence:

- Uses Akka.Persistence for event journaling
- Maintains processing state between restarts
- Implements at-least-once delivery semantics
- Optimizes database operations through batching

## Scaling Considerations

The actor model architecture enables several scaling strategies:

- **Vertical Scaling**: Increase resources on a single node
- **Horizontal Scaling**: Distribute actors across multiple nodes using Akka.Remote
- **Cluster Scaling**: Implement Akka.Cluster for dynamic scaling
- **Sharding**: Use Akka.Cluster.Sharding for tenant and entity distribution

This architecture provides a solid foundation that can grow with your system's needs while maintaining performance and reliability.
