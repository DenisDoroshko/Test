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
    /// Provides access to venue entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFVenueRepository : IRepository<Venue>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFVenueRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFVenueRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Venue entity)
        {
            await _dbContext.Venue.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        public async Task UpdateAsync(Venue entity)
        {
            _dbContext.Venue.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        public async Task DeleteAsync(Venue entity)
        {
            _dbContext.Venue.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets venue entity from Venue table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the venue.</param>
        /// <returns>Venue entity if the operation was successful otherwise null.</returns>
        public async Task<Venue> GetByIdAsync(int id)
        {
            return await _dbContext.Venue.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all venues from Venue table in MSSql database.
        /// </summary>
        /// <returns>Venue list if the operation was successful otherwise empty venue list.</returns>
        public IQueryable<Venue> GetAll()
        {
            return _dbContext.Venue.AsNoTracking();
        }
    }
}
