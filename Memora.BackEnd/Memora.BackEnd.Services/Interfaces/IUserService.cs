using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface IUserService
	{
		public Task<(string accessToken, string refreshToken)?> LoginAsync(string userName, string password);
		public Task<int> RegisterAsync(string userName, string password);
		public Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);
		public Task<List<UserDto>> GetUsersAsync();
		public Task<int> BanUser(Guid userId);
	}
}
