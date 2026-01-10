using EHS.Domain.Entities;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EHS.Application.Repositories
{
    /// <summary>
    /// Generic repository interface defining CRUD operations for all entities.
    /// Located in Application layer per Clean Architecture principles.
    /// Infrastructure layer implements this interface (Dependency Inversion).
    /// </summary>
    /// <typeparam name="T">Entity type inheriting from BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Retrieves all entities asynchronously with optional filtering and includes.
        /// </summary>
        /// <param name="filter">Optional WHERE clause expression</param>
        /// <param name="orderBy">Optional ORDER BY clause using KeyValuePair (property, ascending)</param>
        /// <param name="includeProperties">Comma-separated navigation properties to eager load</param>
        /// <returns>Queryable collection of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Retrieves a single entity by ID asynchronously.
        /// </summary>
        /// <param name="id">Entity ID to retrieve</param>
        /// <param name="includeProperties">Comma-separated navigation properties to eager load</param>
        /// <returns>Entity if found; null otherwise</returns>
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Retrieves entities with pagination support.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="filter">Optional WHERE clause expression</param>
        /// <param name="orderBy">Optional ORDER BY clause</param>
        /// <param name="includeProperties">Comma-separated navigation properties to eager load</param>
        /// <returns>Tuple containing paginated items and total count</returns>
        Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added entity with generated ID</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        void UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity (soft delete by default).
        /// </summary>
        /// <param name="id">Entity ID to delete</param>
        /// <returns>True if deleted; false if not found</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Permanently deletes an entity from the database.
        /// </summary>
        /// <param name="id">Entity ID to permanently delete</param>
        /// <returns>True if deleted; false if not found</returns>
        Task<bool> PermanentDeleteAsync(Guid id);

        /// <summary>
        /// Checks if an entity exists matching the given filter.
        /// </summary>
        /// <param name="filter">WHERE clause expression</param>
        /// <returns>True if entity exists; false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Gets the count of entities matching the filter.
        /// </summary>
        /// <param name="filter">Optional WHERE clause expression</param>
        /// <returns>Count of matching entities</returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        /// <returns>Number of entities affected</returns>
        Task<int> SaveChangesAsync();
    }
}