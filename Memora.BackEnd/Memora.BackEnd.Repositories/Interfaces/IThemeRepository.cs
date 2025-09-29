using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IThemeRepository
	{
		Task<Theme?> GetByProductIdAsync(string productId);
	}
}
