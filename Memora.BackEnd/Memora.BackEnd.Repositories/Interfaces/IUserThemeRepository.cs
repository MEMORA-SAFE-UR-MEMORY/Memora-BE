using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IUserThemeRepository
	{
		Task<bool> UserOwnsThemeAsync(Guid userId, long themeId);
		Task AddAsync(UserTheme userTheme);
	}
}
