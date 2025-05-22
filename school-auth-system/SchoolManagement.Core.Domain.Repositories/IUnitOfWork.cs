using System.Threading.Tasks;
using SchoolManagement.Core.Domain.Authentication;

namespace SchoolManagement.Core.Domain.Repositories
{
    /// <summary>
    /// Unit of Work interface for managing transactions and repositories
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the authentication user repository
        /// </summary>
        IAuthUserRepository AuthUsers { get; }
        
        /// <summary>
        /// Gets the role repository
        /// </summary>
        IRoleRepository Roles { get; }
        
        /// <summary>
        /// Gets the permission repository
        /// </summary>
        IPermissionRepository Permissions { get; }
        
        /// <summary>
        /// Saves all changes made in this unit of work to the database
        /// </summary>
        /// <returns>The number of state entries written to the database</returns>
        Task<int> SaveChangesAsync();
        
        /// <summary>
        /// Begins a transaction
        /// </summary>
        Task BeginTransactionAsync();
        
        /// <summary>
        /// Commits the transaction
        /// </summary>
        Task CommitTransactionAsync();
        
        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
