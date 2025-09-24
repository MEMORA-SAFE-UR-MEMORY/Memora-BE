namespace Memora.BackEnd.Services.Interfaces
{
	public interface IUserService
	{
		public Task<string?> LoginAsync(string userName, string password);
		public Task<int> RegisterAsync(string userName, string password);
	}
}
