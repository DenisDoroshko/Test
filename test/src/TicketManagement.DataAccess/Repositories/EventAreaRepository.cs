using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Repositories
{
    /// <summary>
    /// Provides access to event area entity in MSSql database.
    /// </summary>
    internal class EventAreaRepository : IRepository<EventArea>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAreaRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public EventAreaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">Event area entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(EventArea entity)
        {
            int id = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT EventArea (EventId,Description,CoordX,CoordY,Price,Color) OUTPUT INSERTED.ID " +
                    "VALUES (@EventId,@Description,@CoordX,@CoordY,@Price,@Color)";
                    command.Parameters.AddWithValue("@EventId", entity.EventId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@CoordX", entity.CoordY);
                    command.Parameters.AddWithValue("@CoordY", entity.CoordY);
                    command.Parameters.AddWithValue("@Price", entity.Price);
                    command.Parameters.AddWithValue("@Color", entity.Color);
                    id = (int)await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">Event area entity.</param>
        public async Task UpdateAsync(EventArea entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE EventArea " +
                    $"SET EventId=@EventId,Description=@Description,CoordX=@CoordX,CoordY=@CoordY,Price=@Price,Color=@Color WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@EventId", entity.EventId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@CoordX", entity.CoordY);
                    command.Parameters.AddWithValue("@CoordY", entity.CoordY);
                    command.Parameters.AddWithValue("@Price", entity.Price);
                    command.Parameters.AddWithValue("@Color", entity.Color);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from EventArea table in MSSql database.
        /// </summary>
        /// <param name="entity">EventArea entity.</param>
        public async Task DeleteAsync(EventArea entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM EventArea WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets event area entity from EventArea table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event area.</param>
        /// <returns>Event area entity if the operation was successful otherwise null.</returns>
        public async Task<EventArea> GetByIdAsync(int id)
        {
            EventArea eventArea = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,EventId,Description,CoordX,CoordY,Price,Color FROM EventArea WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            eventArea = new EventArea
                            {
                                Id = (int)reader["Id"],
                                EventId = (int)reader["EventId"],
                                Description = (string)reader["Description"],
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                                Price = (decimal)reader["Price"],
                                Color = (string)reader["Color"],
                            };
                        }
                    }
                }
            }

            return eventArea;
        }

        /// <summary>
        /// Gets all event areas from EventArea table in MSSql database.
        /// </summary>
        /// <returns>Event area list if the operation was successful otherwise empty event area list.</returns>
        public IQueryable<EventArea> GetAll()
        {
            List<EventArea> eventAreas = new List<EventArea>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,EventId,Description,CoordX,CoordY,Price,Color FROM EventArea";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                eventAreas.Add(new EventArea
                                {
                                    Id = (int)reader["Id"],
                                    EventId = (int)reader["EventId"],
                                    Description = (string)reader["Description"],
                                    CoordX = (int)reader["CoordX"],
                                    CoordY = (int)reader["CoordY"],
                                    Price = (decimal)reader["Price"],
                                    Color = (string)reader["Color"],
                                });
                            }
                        }
                    }
                }
            }

            return eventAreas.AsQueryable();
        }
    }
}
