using Memora.BackEnd.Repositories.Entities;
using Memora.BackEnd.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly string _connectionString;

		public UserRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<User?> GetByUsernameAsync(string userName)
		{
			await using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			const string query = "SELECT id, username, password_hash, role_id FROM users WHERE username = @username LIMIT 1;";
			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("username", userName);

			await using var reader = await command.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				var id = reader.GetGuid(0);
				var userNameResult = reader.GetString(1);
				var passwordHash = reader.GetString(2);
				var roleId = reader.GetInt16(3);
				return new User(id, userNameResult, passwordHash, roleId);
			}
			return null;
		}

		public async Task CreateUserAsync(User user)
		{
			await using var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();

			const string query = "INSERT INTO users (username, password_hash, role_id) VALUES (@username, @password_hash, @role_id);";
			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("username", user.UserName);
			command.Parameters.AddWithValue("password_hash", user.PasswordHash);
			command.Parameters.AddWithValue("role_id", user.RoleId);
			await command.ExecuteNonQueryAsync();
		}
	}
}
