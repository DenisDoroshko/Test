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
    /// Provides access to area entity in MSSql database.
    /// </summary>
    internal class AreaRepository : IRepository<Area>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public AreaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Area entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT Area (LayoutId,Description,CoordX,CoordY,Width,Height,Color) OUTPUT INSERTED.ID " +
                        "VALUES (@LayoutId,@Description,@CoordX,@CoordY,@Width,@Height,@Color)";
                    command.Parameters.AddWithValue("@LayoutId", entity.LayoutId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@CoordX", entity.CoordX);
                    command.Parameters.AddWithValue("@CoordY", entity.CoordY);
                    command.Parameters.AddWithValue("@Width", entity.Width);
                    command.Parameters.AddWithValue("@Height", entity.Height);
                    command.Parameters.AddWithValue("@Color", entity.Color);
                    id = (int)await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        public async Task UpdateAsync(Area entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Area " +
                    $"SET LayoutId=@LayoutId,Description=@Description,CoordX=@CoordX," +
                    $"CoordY=@CoordY,Width=Width,Height=Height,Color=Color WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@LayoutId", entity.LayoutId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@CoordX", entity.CoordY);
                    command.Parameters.AddWithValue("@CoordY", entity.CoordY);
                    command.Parameters.AddWithValue("@Width", entity.Width);
                    command.Parameters.AddWithValue("@Height", entity.Height);
                    command.Parameters.AddWithValue("@Color", entity.Color);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from Area table in MSSql database.
        /// </summary>
        /// <param name="entity">Area entity.</param>
        public async Task DeleteAsync(Area entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Area WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets area entity from Area table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the area.</param>
        /// <returns>Area entity if the operation was successful otherwise null.</returns>
        public async Task<Area> GetByIdAsync(int id)
        {
            Area area = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,LayoutId,Description,CoordX,CoordY,Width,Height,Color FROM Area WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            area = new Area
                            {
                                Id = (int)reader["Id"],
                                LayoutId = (int)reader["LayoutId"],
                                Description = (string)reader["Description"],
                                CoordX = (int)reader["CoordX"],
                                CoordY = (int)reader["CoordY"],
                                Width = (int)reader["Width"],
                                Height = (int)reader["Height"],
                                Color = (string)reader["Color"],
                            };
                        }
                    }
                }
            }

            return area;
        }

        /// <summary>
        /// Gets all areas from Area table in MSSql database.
        /// </summary>
        /// <returns>Area list if the operation was successful otherwise empty area list.</returns>
        public IQueryable<Area> GetAll()
        {
            List<Area> areas = new List<Area>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,LayoutId,Description,CoordX,CoordY,Width,Height,Color FROM Area";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                areas.Add(new Area
                                {
                                    Id = (int)reader["Id"],
                                    LayoutId = (int)reader["LayoutId"],
                                    Description = (string)reader["Description"],
                                    CoordX = (int)reader["CoordX"],
                                    CoordY = (int)reader["CoordY"],
                                    Width = (int)reader["Width"],
                                    Height = (int)reader["Height"],
                                    Color = (string)reader["Color"],
                                });
                            }
                        }
                    }
                }
            }

            return areas.AsQueryable();
        }
    }
}
