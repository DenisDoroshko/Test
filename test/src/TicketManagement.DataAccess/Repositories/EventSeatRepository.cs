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
    /// Provides access to event seat entity in MSSql database.
    /// </summary>
    internal class EventSeatRepository : IRepository<EventSeat>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSeatRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public EventSeatRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to event seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(EventSeat entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT EventSeat (EventAreaId,Row,Number,State,UserId) OUTPUT INSERTED.ID " +
                    "VALUES (@EventAreaId,@Row,@Number,@State,@UserId)";
                    command.Parameters.AddWithValue("@EventAreaId", entity.EventAreaId);
                    command.Parameters.AddWithValue("@Row", entity.Row);
                    command.Parameters.AddWithValue("@Number", entity.Number);
                    command.Parameters.AddWithValue("@State", entity.State);
                    command.Parameters.AddWithValue("@UserId", (object)entity.UserId ?? DBNull.Value);
                    id = (int)await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in EventSeat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        public async Task UpdateAsync(EventSeat entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE EventSeat SET Row=@Row,Number=@Number,State=@State,UserId=@UserId WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@Row", entity.Row);
                    command.Parameters.AddWithValue("@Number", entity.Number);
                    command.Parameters.AddWithValue("@State", entity.State);
                    command.Parameters.AddWithValue("@UserId", (object)entity.UserId ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from EventSeat table in MSSql database.
        /// </summary>
        /// <param name="entity">Event seat entity.</param>
        public async Task DeleteAsync(EventSeat entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM EventSeat WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets seat entity from EventSeat table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event seat.</param>
        /// <returns>Event seat entity if the operation was successful otherwise null.</returns>
        public async Task<EventSeat> GetByIdAsync(int id)
        {
            EventSeat eventSeat = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,EventAreaId,Row,Number,State,UserId FROM EventSeat WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            eventSeat = new EventSeat
                            {
                                Id = (int)reader["Id"],
                                EventAreaId = (int)reader["EventAreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                                State = (int)reader["State"],
                                UserId = reader.IsDBNull(5) ? null : (string)reader["UserId"],
                            };
                        }
                    }
                }
            }

            return eventSeat;
        }

        /// <summary>
        /// Gets all event seats from EventSeat table in MSSql database.
        /// </summary>
        /// <returns>Event seat list if the operation was successful otherwise empty event seat list.</returns>
        public IQueryable<EventSeat> GetAll()
        {
            List<EventSeat> eventSeats = new List<EventSeat>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,EventAreaId,Row,Number,State,UserId FROM EventSeat";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                eventSeats.Add(new EventSeat
                                {
                                    Id = (int)reader["Id"],
                                    EventAreaId = (int)reader["EventAreaId"],
                                    Row = (int)reader["Row"],
                                    Number = (int)reader["Number"],
                                    State = (int)reader["State"],
                                    UserId = reader.IsDBNull(5) ? null : (string)reader["UserId"],
                                });
                            }
                        }
                    }
                }
            }

            return eventSeats.AsQueryable();
        }
    }
}
