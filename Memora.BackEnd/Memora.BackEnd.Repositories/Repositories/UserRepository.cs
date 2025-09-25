using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly PostgresContext _context;

		public UserRepository(PostgresContext context) => _context = context;

		public async Task<User?> GetByUsernameAsync(string userName)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
			return user;
		}

		public async Task<int> CreateUserAsync(User user)
		{
			await _context.Users.AddAsync(user);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> UpdateUserAsync(User user)
		{
			_context.Users.Update(user);
			return await _context.SaveChangesAsync();
		}

		public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
		}
	}
}
