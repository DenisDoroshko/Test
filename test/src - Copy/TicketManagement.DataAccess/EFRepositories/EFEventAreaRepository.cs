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
    /// Provides access to event area entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFEventAreaRepository : IRepository<EventArea>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFEventAreaRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFEventAreaRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">Event area entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(EventArea entity)
        {
            await _dbContext.EventArea.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">Event area entity.</param>
        public async Task UpdateAsync(EventArea entity)
        {
            _dbContext.EventArea.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">EventArea entity.</param>
        public async Task DeleteAsync(EventArea entity)
        {
            _dbContext.EventArea.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets event area entity from EventArea table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event area.</param>
        /// <returns>Event area entity if the operation was successful otherwise null.</returns>
        public async Task<EventArea> GetByIdAsync(int id)
        {
            return await _dbContext.EventArea.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all event areas from EventArea table in MSSql database.
        /// </summary>
        /// <returns>Event area list if the operation was successful otherwise empty event area list.</returns>
        public IQueryable<EventArea> GetAll()
        {
            return _dbContext.EventArea.AsNoTracking();
        }
    }
}
