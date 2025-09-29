using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class ThemeRepository : IThemeRepository
	{
		private readonly PostgresContext _context;

		public ThemeRepository(PostgresContext context)
		{
			_context = context;
		}

		public async Task<Theme?> GetByProductIdAsync(string productId)
		{
			return await _context.Themes.Include(d => d.Door)
				.FirstOrDefaultAsync(t => t.RevenueCatProductId == productId);
		}
	}
}
