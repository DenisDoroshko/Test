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
    /// Provides access to seat entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFSeatRepository : IRepository<Seat>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFSeatRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFSeatRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Seat entity)
        {
            await _dbContext.Seat.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        public async Task UpdateAsync(Seat entity)
        {
            _dbContext.Seat.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        public async Task DeleteAsync(Seat entity)
        {
            _dbContext.Seat.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets seat entity from Seat table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the seat.</param>
        /// <returns>Seat entity if the operation was successful otherwise null.</returns>
        public async Task<Seat> GetByIdAsync(int id)
        {
            return await _dbContext.Seat.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all seats from Seat table in MSSql database.
        /// </summary>
        /// <returns>Seat list if the operation was successful otherwise empty seat list.</returns>
        public IQueryable<Seat> GetAll()
        {
            return _dbContext.Seat.AsNoTracking();
        }
    }
}
