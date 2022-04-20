using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EFRepositories
{
    /// <summary>
    /// Provides access to event entity in MSSql database via Entity Framework.
    /// </summary>
    internal class EFEventRepository : IRepository<Event>
    {
        private readonly TicketManagementContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Access to the database.</param>
        public EFEventRepository(TicketManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds entity to Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Event entity)
        {
            var procedureParams = new object[]
            {
                new Microsoft.Data.SqlClient.SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output,
                },
                new Microsoft.Data.SqlClient.SqlParameter("@Name", entity.Name),
                new Microsoft.Data.SqlClient.SqlParameter("@Description", entity.Description),
                new Microsoft.Data.SqlClient.SqlParameter("@Start", entity.Start),
                new Microsoft.Data.SqlClient.SqlParameter("@Finish", entity.Finish),
                new Microsoft.Data.SqlClient.SqlParameter("@LayoutId", entity.LayoutId),
                new Microsoft.Data.SqlClient.SqlParameter("@Image", (object)entity.Image ?? DBNull.Value),
            };

            await _dbContext.Database.ExecuteSqlRawAsync($"CreateEvent @Id OUT, @Name, @Description, @Start, @Finish, @LayoutId,@Image", procedureParams);
            return (int)((Microsoft.Data.SqlClient.SqlParameter)procedureParams[0]).Value;
        }

        /// <summary>
        /// Updates entity in Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        public async Task UpdateAsync(Event entity)
        {
            var procedureParams = new object[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@Id", entity.Id),
                new Microsoft.Data.SqlClient.SqlParameter("@Name", entity.Name),
                new Microsoft.Data.SqlClient.SqlParameter("@Description", entity.Description),
                new Microsoft.Data.SqlClient.SqlParameter("@Start", entity.Start),
                new Microsoft.Data.SqlClient.SqlParameter("@Finish", entity.Finish),
                new Microsoft.Data.SqlClient.SqlParameter("@LayoutId", entity.LayoutId),
                new Microsoft.Data.SqlClient.SqlParameter("@Image", (object)entity.Image ?? DBNull.Value),
            };

            await _dbContext.Database.ExecuteSqlRawAsync($"UpdateEvent @Id, @Name, @Description, @Start, @Finish, @LayoutId, @Image", procedureParams);
        }

        /// <summary>
        /// Deletes entity from Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        public async Task DeleteAsync(Event entity)
        {
            var idParam = new Microsoft.Data.SqlClient.SqlParameter("@Id", entity.Id);
            await _dbContext.Database.ExecuteSqlRawAsync($"DeleteEvent @Id", idParam);
        }

        /// <summary>
        /// Gets event entity from Event table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event.</param>
        /// <returns>Event entity if the operation was successful otherwise null.</returns>
        public async Task<Event> GetByIdAsync(int id)
        {
            return await _dbContext.Event.FindAsync(id);
        }

        /// <summary>
        /// Gets all events from Event table in MSSql database.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        public IQueryable<Event> GetAll()
        {
            return _dbContext.Event.AsNoTracking();
        }
    }
}
