using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class UserThemeRepository : IUserThemeRepository
	{
		private readonly PostgresContext _context;

		public UserThemeRepository(PostgresContext context)
		{
			_context = context;
		}

		public async Task AddAsync(UserTheme userTheme)
		{
			await _context.UserThemes.AddAsync(userTheme);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> UserOwnsThemeAsync(Guid userId, long themeId)
		{
			return await _context.UserThemes
				.AnyAsync(ut => ut.UserId == userId && ut.ThemeId == themeId);
		}
	}
}
