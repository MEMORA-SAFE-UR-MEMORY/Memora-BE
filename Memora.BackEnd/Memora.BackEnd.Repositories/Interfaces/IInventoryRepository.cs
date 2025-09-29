using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IInventoryRepository
	{
		Task<Inventory> GetOrCreateByUserIdAsync(Guid userId);
		Task AddOrUpdateItemsInInventoryAsync(long inventoryId, IEnumerable<long> itemIds, int quantityToAdd = 1);
	}
}