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
    /// Provides access to seat entity in MSSql database.
    /// </summary>
    internal class SeatRepository : IRepository<Seat>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public SeatRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Seat entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT Seat (AreaId,Row,Number) OUTPUT INSERTED.ID " +
                    "VALUES (@AreaId,@Row,@Number)";
                    command.Parameters.AddWithValue("@AreaId", entity.AreaId);
                    command.Parameters.AddWithValue("@Row", entity.Row);
                    command.Parameters.AddWithValue("@Number", entity.Number);
                    id = (int) await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        public async Task UpdateAsync(Seat entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Seat SET AreaId=@AreaId,Row=@Row,Number=@Number WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@AreaId", entity.AreaId);
                    command.Parameters.AddWithValue("@Row", entity.Row);
                    command.Parameters.AddWithValue("@Number", entity.Number);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from Seat table in MSSql database.
        /// </summary>
        /// <param name="entity">Seat entity.</param>
        public async Task DeleteAsync(Seat entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Seat WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets seat entity from Seat table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the seat.</param>
        /// <returns>Seat entity if the operation was successful otherwise null.</returns>
        public async Task<Seat> GetByIdAsync(int id)
        {
            Seat seat = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,AreaId,Row,Number FROM Seat WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            seat = new Seat
                            {
                                Id = (int)reader["Id"],
                                AreaId = (int)reader["AreaId"],
                                Row = (int)reader["Row"],
                                Number = (int)reader["Number"],
                            };
                        }
                    }
                }
            }

            return seat;
        }

        /// <summary>
        /// Gets all seats from Seat table in MSSql database.
        /// </summary>
        /// <returns>Seat list if the operation was successful otherwise empty seat list.</returns>
        public IQueryable<Seat> GetAll()
        {
            List<Seat> seats = new List<Seat>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,AreaId,Row,Number FROM Seat";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                seats.Add(new Seat
                                {
                                    Id = (int)reader["Id"],
                                    AreaId = (int)reader["AreaId"],
                                    Row = (int)reader["Row"],
                                    Number = (int)reader["Number"],
                                });
                            }
                        }
                    }
                }
            }

            return seats.AsQueryable();
        }
    }
}
