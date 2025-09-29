using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByUsernameAsync(string userName);
		Task<int> CreateUserAsync(User user);
		Task<int> UpdateUserAsync(User user);
		Task<User?> GetByRefreshTokenAsync(string refreshToken);
		Task<List<User>> GetAllUsersAsync();
		Task<int> BanUser(Guid userId);
    }
}
