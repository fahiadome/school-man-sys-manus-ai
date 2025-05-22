using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Core.Domain.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation for managing transactions and repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        private IAuthUserRepository _authUserRepository;
        private IRoleRepository _roleRepository;
        private IPermissionRepository _permissionRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the authentication user repository
        /// </summary>
        public IAuthUserRepository AuthUsers => 
            _authUserRepository ??= new AuthUserRepository(_context);

        /// <summary>
        /// Gets the role repository
        /// </summary>
        public IRoleRepository Roles => 
            _roleRepository ??= new RoleRepository(_context);

        /// <summary>
        /// Gets the permission repository
        /// </summary>
        public IPermissionRepository Permissions => 
            _permissionRepository ??= new PermissionRepository(_context);

        /// <summary>
        /// Saves all changes made in this unit of work to the database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the transaction
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            finally
            {
                await _transaction?.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                await _transaction?.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the context and transaction
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the context and transaction
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
