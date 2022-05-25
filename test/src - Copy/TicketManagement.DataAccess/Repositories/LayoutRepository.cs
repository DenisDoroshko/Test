using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.Repositories
{
    /// <summary>
    /// Provides access to layout entity in MSSql database.
    /// </summary>
    internal class LayoutRepository : IRepository<Layout>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutRepository"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public LayoutRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Adds entity to Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        /// <returns>Added entity id if the operation was successful otherwise zero.</returns>
        public async Task<int> CreateAsync(Layout entity)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT Layout (Name,VenueId,Description,Width,Height)" +
                        " OUTPUT INSERTED.ID VALUES (@Name,@VenueId,@Description,@Width,@Height)";
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@VenueId", entity.VenueId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Width", entity.Width);
                    command.Parameters.AddWithValue("@Height", entity.Height);
                    id = (int)await command.ExecuteScalarAsync();
                }
            }

            return id;
        }

        /// <summary>
        /// Updates entity in Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        public async Task UpdateAsync(Layout entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Layout SET Name=@Name,VenueId=@VenueId," +
                        $"Description=@Description,Width=@Width,Height=@Height WHERE Id={entity.Id}";
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@VenueId", entity.VenueId);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Width", entity.Width);
                    command.Parameters.AddWithValue("@Height", entity.Height);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Deletes entity from Layout table in MSSql database.
        /// </summary>
        /// <param name="entity">Layout entity.</param>
        public async Task DeleteAsync(Layout entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Layout WHERE Id={entity.Id}";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets layout entity from Layout table in MSSql database by specified id.
        /// </summary>
        /// <param name="id">Specified id of the layout.</param>
        /// <returns>Layout entity if the operation was successful otherwise null.</returns>
        public async Task<Layout> GetByIdAsync(int id)
        {
            Layout layout = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,VenueId,Description,Width,Height FROM Layout WHERE Id={id}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            layout = new Layout
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                VenueId = (int)reader["VenueId"],
                                Description = (string)reader["Description"],
                                Width = (int)reader["Width"],
                                Height = (int)reader["Height"],
                            };
                        }
                    }
                }
            }

            return layout;
        }

        /// <summary>
        /// Gets all layouts from Layout table in MSSql database.
        /// </summary>
        /// <returns>Layout list if the operation was successful otherwise empty layout list.</returns>
        public IQueryable<Layout> GetAll()
        {
            List<Layout> layouts = new List<Layout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id,Name,VenueId,Description,Width,Height FROM Layout";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                layouts.Add(new Layout
                                {
                                    Id = (int)reader["Id"],
                                    Name = (string)reader["Name"],
                                    VenueId = (int)reader["VenueId"],
                                    Description = (string)reader["Description"],
                                    Width = (int)reader["Width"],
                                    Height = (int)reader["Height"],
                                });
                            }
                        }
                    }
                }
            }

            return layouts.AsQueryable();
        }
    }
}
