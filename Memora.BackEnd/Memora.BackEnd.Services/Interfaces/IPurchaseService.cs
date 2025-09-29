namespace Memora.BackEnd.Services.Interfaces
{
	public interface IPurchaseService
	{
		Task GrantThemeAccessAsync(string appUserId, string productId);
	}
}
