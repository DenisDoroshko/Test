using System.Collections.Generic;

namespace ThirdPartyEventEditor.Data
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
        int Create(T entity);

        /// <summary>
        /// Updates entity in data storage.
        /// </summary>
        /// <param name="entity">T entity.</param>
        void Update(T entity);

        /// <summary>
        /// Deletes entity from data storage.
        /// </summary>
        /// <param name="entity">T entity.</param>
        void Delete(T entity);

        /// <summary>
        /// Gets T entity from data storage by specified id.
        /// </summary>
        /// <param name="id">Specified id of the T entity.</param>
        /// <returns>T entity if the operation was successful otherwise null.</returns>
        T GetById(int id);

        /// <summary>
        /// Gets all T entities from data storage.
        /// </summary>
        /// <returns>T list if the operation was successful otherwise empty T list.</returns>
        List<T> GetAll();
    }
}
