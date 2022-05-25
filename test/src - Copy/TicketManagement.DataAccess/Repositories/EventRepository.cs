using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Repositories
{
    /// <summary>
    /// Provides access to event entity in MSSql database.
    /// </summary>
    internal class EventRepository : IRepository<Event>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Event entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("CreateEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var idParameter = new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output,
                    };

                    command.Parameters.Add(idParameter);
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Start", entity.Start);
                    command.Parameters.AddWithValue("@Finish", entity.Finish);
                    command.Parameters.AddWithValue("@LayoutId", entity.LayoutId);
                    command.Parameters.AddWithValue("@Image", entity.Image);
                    await command.ExecuteNonQueryAsync();
                    id = (int)idParameter.Value;
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        public async Task UpdateAsync(Event entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("UpdateEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Start", entity.Start);
                    command.Parameters.AddWithValue("@Finish", entity.Finish);
                    command.Parameters.AddWithValue("@LayoutId", entity.LayoutId);
                    command.Parameters.AddWithValue("@Image", entity.Image);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from Event table in MSSql database.
        /// </summary>
        /// <param name="entity">Event entity.</param>
        public async Task DeleteAsync(Event entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DeleteEvent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets event entity from Event table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the event.</param>
        /// <returns>Event entity if the operation was successful otherwise null.</returns>
        public async Task<Event> GetByIdAsync(int id)
        {
            Event eventEntity = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,Description,Start,Finish,LayoutId,Image " +
                    $"FROM Event WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            eventEntity = new Event
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Description = (string)reader["Description"],
                                Start = (DateTime)reader["Start"],
                                Finish = (DateTime)reader["Finish"],
                                LayoutId = (int)reader["LayoutId"],
                                Image = (string)reader["Image"],
                            };
                        }
                    }
                }
            }

            return eventEntity;
        }

        /// <summary>
        /// Gets all events from Event table in MSSql database.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        public IQueryable<Event> GetAll()
        {
            List<Event> events = new List<Event>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,Description,Start,Finish,LayoutId,Image FROM Event";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                events.Add(new Event
                                {
                                    Id = (int)reader["Id"],
                                    Name = (string)reader["Name"],
                                    Description = (string)reader["Description"],
                                    Start = (DateTime)reader["Start"],
                                    Finish = (DateTime)reader["Finish"],
                                    LayoutId = (int)reader["LayoutId"],
                                    Image = (string)reader["Image"],
                                });
                            }
                        }
                    }
                }
            }

            return events.AsQueryable();
        }
    }
}
