using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Domain.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<TEntity> GetByIdAsync(string id);
        
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>A collection of all entities</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        /// <summary>
        /// Finds entities based on a predicate
        /// </summary>
        /// <param name="predicate">The filter predicate</param>
        /// <returns>A collection of entities matching the predicate</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        
        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        Task<TEntity> AddAsync(TEntity entity);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        Task UpdateAsync(TEntity entity);
        
        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        Task DeleteAsync(TEntity entity);
        
        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        Task DeleteByIdAsync(string id);
        
        /// <summary>
        /// Checks if any entity matches the predicate
        /// </summary>
        /// <param name="predicate">The filter predicate</param>
        /// <returns>True if any entity matches, false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        
        /// <summary>
        /// Counts entities matching the predicate
        /// </summary>
        /// <param name="predicate">The filter predicate</param>
        /// <returns>The count of matching entities</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    }
}
