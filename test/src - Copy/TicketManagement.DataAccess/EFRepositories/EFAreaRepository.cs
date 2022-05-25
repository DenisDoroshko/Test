using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EFRepositories
{
    /// <summary>
    /// Provides access to area entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFAreaRepository : IRepository<Area>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFAreaRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to database.</param>
        public EFAreaRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Area entity)
        {
            await _dbContext.Area.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        public async Task UpdateAsync(Area entity)
        {
            _dbContext.Area.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        public async Task DeleteAsync(Area entity)
        {
            _dbContext.Area.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets area entity from Area table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the area.</param>
        /// <returns>Area entity if the operation was successful otherwise null.</returns>
        public async Task<Area> GetByIdAsync(int id)
        {
            return await _dbContext.Area.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all areas from Area table in MSSql database.
        /// </summary>
        /// <returns>Area list if the operation was successful otherwise empty area list.</returns>
        public IQueryable<Area> GetAll()
        {
            return _dbContext.Area.AsNoTracking();
        }
    }
}
