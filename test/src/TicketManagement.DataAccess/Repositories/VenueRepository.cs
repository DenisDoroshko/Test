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
    /// Provides access to venue entity in MSSql database.
    /// </summary>
    internal class VenueRepository : IRepository<Venue>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public VenueRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Venue entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT Venue (Name,Description,Address,Phone) OUTPUT INSERTED.ID " +
                    "VALUES (@Name,@Description,@Address,@Phone)";
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Address", entity.Address);
                    command.Parameters.AddWithValue("@Phone", entity.Phone);
                    id = (int)await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        public async Task UpdateAsync(Venue entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Venue SET Name=@Name, Description=@Description,Address=@Address, Phone=@Phone" +
                    $" WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Address", entity.Address);
                    command.Parameters.AddWithValue("@Phone", entity.Phone);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from Venue table in MSSql database.
        /// </summary>
        /// <param name="entity">Venue entity.</param>
        public async Task DeleteAsync(Venue entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Venue WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets venue entity from Venue table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the venue.</param>
        /// <returns>Venue entity if the operation was successful otherwise null.</returns>
        public async Task<Venue> GetByIdAsync(int id)
        {
            Venue venue = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,Description,Address,Phone FROM Venue WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            venue = new Venue
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Description = (string)reader["Description"],
                                Address = (string)reader["Address"],
                                Phone = (string)reader["Phone"],
                            };
                        }
                    }
                }
            }

            return venue;
        }

        /// <summary>
        /// Gets all venues from Venue table in MSSql database.
        /// </summary>
        /// <returns>Venue list if the operation was successful otherwise empty venue list.</returns>
        public IQueryable<Venue> GetAll()
        {
            List<Venue> venues = new List<Venue>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,Description,Address,Phone FROM Venue";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                venues.Add(new Venue
                                {
                                    Id = (int)reader["Id"],
                                    Name = (string)reader["Name"],
                                    Description = (string)reader["Description"],
                                    Address = (string)reader["Address"],
                                    Phone = (string)reader["Phone"],
                                });
                            }
                        }
                    }
                }
            }

            return venues.AsQueryable();
        }
    }
}
