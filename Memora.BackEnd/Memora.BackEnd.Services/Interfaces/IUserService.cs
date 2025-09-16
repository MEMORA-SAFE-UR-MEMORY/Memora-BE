namespace Memora.BackEnd.Services.Interfaces
{
	public interface IUserService
	{
		public Task<string?> LoginAsync(string userName, string password);
		public Task RegisterAsync(string userName, string password);
	}
}
