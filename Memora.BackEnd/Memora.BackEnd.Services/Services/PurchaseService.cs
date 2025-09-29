using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Interfaces;

namespace Memora.BackEnd.Services.Services
{
	public class PurchaseService : IPurchaseService
	{
		private readonly IUserRepository _userRepository;
		private readonly IThemeRepository _themeRepository;
		private readonly IUserThemeRepository _userThemeRepository;

		public PurchaseService(IUserRepository userRepository, IThemeRepository themeRepository, IUserThemeRepository userThemeRepository)
		{
			_userRepository = userRepository;
			_themeRepository = themeRepository;
			_userThemeRepository = userThemeRepository;
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
				throw new KeyNotFoundException($"User with ID '{appUserId}' not found.");
			}

			var theme = await _themeRepository.GetByProductIdAsync(productId);
			if (theme == null)
			{
				throw new KeyNotFoundException($"Theme with Product ID '{productId}' not found.");
			}

			bool alreadyOwns = await _userThemeRepository.UserOwnsThemeAsync(user.Id, theme.Id);
			if (alreadyOwns)
			{
				return;
			}

			var newUserTheme = new UserTheme
			{
				UserId = user.Id,
				ThemeId = theme.Id,
				CreatedAt = DateTime.UtcNow
			};

			await _userThemeRepository.AddAsync(newUserTheme);
		}
	}
}
