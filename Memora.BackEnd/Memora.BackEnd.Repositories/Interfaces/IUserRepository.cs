using Memora.BackEnd.Repositories.Entities;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByUsernameAsync(string userName);
		Task CreateUserAsync(User user);
	}
}
