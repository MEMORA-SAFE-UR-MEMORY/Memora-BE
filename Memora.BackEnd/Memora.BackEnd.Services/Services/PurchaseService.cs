using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Memora.BackEnd.Services.Services
{
	public class PurchaseService : IPurchaseService
	{
		private readonly IUserRepository _userRepository;
		private readonly IThemeRepository _themeRepository;
		private readonly IUserThemeRepository _userThemeRepository;
		private readonly IItemRepository _itemRepository;
		private readonly IInventoryRepository _inventoryRepository;
		private readonly ILogger<PurchaseService> _logger;

		public PurchaseService(
			IUserRepository userRepository,
			IThemeRepository themeRepository,
			IUserThemeRepository userThemeRepository,
			IItemRepository itemRepository,
			IInventoryRepository inventoryRepository,
			ILogger<PurchaseService> logger)
		{
			_userRepository = userRepository;
			_themeRepository = themeRepository;
			_userThemeRepository = userThemeRepository;
			_itemRepository = itemRepository;
			_inventoryRepository = inventoryRepository;
			_logger = logger;
		}

		public async Task GrantThemeAccessAsync(string appUserId, string productId)
		{
			if (!Guid.TryParse(appUserId, out var userId))
			{
				throw new ArgumentException("Invalid user ID format.", nameof(appUserId));
			}

			var user = await _userRepository.GetByUsernameAsync(userId.ToString());
			if (user == null)
			{
				_logger.LogWarning("User with AppUserID {AppUserId} not found in database.", appUserId);
				throw new KeyNotFoundException($"User with ID '{appUserId}' not found.");
			}

			var theme = await _themeRepository.GetByProductIdAsync(productId);
			if (theme == null)
			{
				_logger.LogWarning("Theme with RevenueCat ProductID {ProductId} not found in database.", productId);
				throw new KeyNotFoundException($"Theme with Product ID '{productId}' not found.");
			}

			bool alreadyOwns = await _userThemeRepository.UserOwnsThemeAsync(user.Id, theme.Id);
			if (!alreadyOwns)
			{
				var newUserTheme = new UserTheme
				{
					UserId = user.Id,
					ThemeId = theme.Id,
					CreatedAt = DateTime.UtcNow
				};
				await _userThemeRepository.AddAsync(newUserTheme);
				_logger.LogInformation("Granted theme ownership for Product ID {ProductId} to User {UserId}.", productId, user.Id);
			}
			else
			{
				_logger.LogInformation("User {UserId} already owns theme with Product ID {ProductId}. Skipping theme grant.", user.Id, productId);
			}

			await GrantThemeItemsToUserInventory(user, theme);
		}

		private async Task GrantThemeItemsToUserInventory(User user, Theme theme)
		{
			var itemsInTheme = await _itemRepository.GetItemsByThemeIdAsync(theme.Id);
			if (!itemsInTheme.Any())
			{
				_logger.LogInformation("Theme {ThemeId} has no associated items to grant.", theme.Id);
				return;
			}

			var inventory = await _inventoryRepository.GetOrCreateByUserIdAsync(user.Id);

			var itemIds = itemsInTheme.Select(item => item.Id);
			await _inventoryRepository.AddOrUpdateItemsInInventoryAsync(inventory.Id, itemIds, 1);

			_logger.LogInformation("Successfully processed {ItemCount} items from theme {ThemeId} for user {UserId}'s inventory.", itemsInTheme.Count, theme.Id, user.Id);
		}
	}
}