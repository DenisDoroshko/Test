using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EFRepositories
{
    /// <summary>
    /// Provides access to event seat entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFEventSeatRepository : IRepository<EventSeat>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFEventSeatRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFEventSeatRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to event seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(EventSeat entity)
        {
            await _dbContext.EventSeat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in EventSeat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        public async Task UpdateAsync(EventSeat entity)
        {
            _dbContext.EventSeat.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from EventSeat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        public async Task DeleteAsync(EventSeat entity)
        {
            _dbContext.EventSeat.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets seat entity from EventSeat table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event seat.</param>
        /// <returns>Event seat entity if the operation was successful otherwise null.</returns>
        public async Task<EventSeat> GetByIdAsync(int id)
        {
            return await _dbContext.EventSeat.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all event seats from EventSeat table in MSSql database.
        /// </summary>
        /// <returns>Event seat list if the operation was successful otherwise empty event seat list.</returns>
        public IQueryable<EventSeat> GetAll()
        {
            return _dbContext.EventSeat.AsNoTracking();
        }
    }
}
