// Memora.BackEnd/Memora.BackEnd.Repositories/Repositories/InventoryRepository.cs
using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class InventoryRepository : IInventoryRepository
	{
		private readonly PostgresContext _context;

		public InventoryRepository(PostgresContext context)
		{
			_context = context;
		}

		public async Task<Inventory> GetOrCreateByUserIdAsync(Guid userId)
		{
			var inventory = await _context.Inventories
				.AsTracking()
				.FirstOrDefaultAsync(i => i.UserId == userId);

			if (inventory == null)
			{
				inventory = new Inventory { UserId = userId, CreatedAt = DateTime.UtcNow };
				await _context.Inventories.AddAsync(inventory);
				await _context.SaveChangesAsync();
			}

			return inventory;
		}

		public async Task AddOrUpdateItemsInInventoryAsync(long inventoryId, IEnumerable<long> itemIds, int quantityToAdd = 1)
		{
			var existingInventoryItems = await _context.InventoryItems
				.Where(ii => ii.InventoryId == inventoryId && itemIds.Contains(ii.ItemId))
				.ToListAsync();

			var existingItemIds = existingInventoryItems.Select(ii => ii.ItemId).ToHashSet();
			var itemsToUpdate = new List<InventoryItem>();
			var itemsToAdd = new List<InventoryItem>();

			foreach (var existingItem in existingInventoryItems)
			{
				existingItem.Quantity += quantityToAdd;
				itemsToUpdate.Add(existingItem);
			}

			foreach (var itemId in itemIds)
			{
				if (!existingItemIds.Contains(itemId))
				{
					itemsToAdd.Add(new InventoryItem
					{
						InventoryId = inventoryId,
						ItemId = itemId,
						Quantity = quantityToAdd,
						CreatedAt = DateTime.UtcNow
					});
				}
			}

			if (itemsToUpdate.Any())
			{
				_context.InventoryItems.UpdateRange(itemsToUpdate);
			}

			if (itemsToAdd.Any())
			{
				await _context.InventoryItems.AddRangeAsync(itemsToAdd);
			}

			if (itemsToUpdate.Any() || itemsToAdd.Any())
			{
				await _context.SaveChangesAsync();
			}
		}
	}
}