using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagement.DataAccess.Interfaces
{
    /// <summary>
    /// Provides access to T entity in data storage.
    /// </summary>
    public interface IRepository<T>
    {
        /// <summary>
        /// Adds entity to data storage.
        /// </summary>
        /// <param name="entity">T entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        Task<int> CreateAsync(T entity);

        /// <summary>
        /// Updates entity in data storage.
        /// </summary>
        /// <param name="entity">T entity.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes entity from data storage.
        /// </summary>
        /// <param name="entity">T entity.</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Gets T entity from data storage by specified id.
        /// </summary>
        /// <param name="id">Specified id of the T entity.</param>
        /// <returns>T entity if the operation was successful otherwise null.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Gets all T entities from data storage.
        /// </summary>
        /// <returns>T list if the operation was successful otherwise empty T list.</returns>
        IQueryable<T> GetAll();
    }
}
