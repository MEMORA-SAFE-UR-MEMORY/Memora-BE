using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class ItemRepository : IItemRepository
	{
		private readonly PostgresContext _context;

		public ItemRepository(PostgresContext context)
		{
			_context = context;
		}

		public async Task<List<Item>> GetItemsByThemeIdAsync(long themeId)
		{
			return await _context.Items.Include(t => t.Theme)	
				.Where(i => i.ThemeId == themeId)
				.ToListAsync();
		}
	}
}
