using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IItemRepository
	{
		Task<List<Item>> GetItemsByThemeIdAsync(long themeId);
	}
}
