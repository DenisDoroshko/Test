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
    /// Provides access to layout entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFLayoutRepository : IRepository<Layout>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFLayoutRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFLayoutRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Layout entity)
        {
            await _dbContext.Layout.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Updates entity in Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        public async Task UpdateAsync(Layout entity)
        {
            _dbContext.Layout.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes entity from Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        public async Task DeleteAsync(Layout entity)
        {
            _dbContext.Layout.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets layout entity from Layout table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the layout.</param>
        /// <returns>Layout entity if the operation was successful otherwise null.</returns>
        public async Task<Layout> GetByIdAsync(int id)
        {
            return await _dbContext.Layout.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all layouts from Layout table in MSSql database.
        /// </summary>
        /// <returns>Layout list if the operation was successful otherwise empty layout list.</returns>
        public IQueryable<Layout> GetAll()
        {
            return _dbContext.Layout.AsNoTracking();
        }
    }
}
