using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IOrderRepository
    {
		Task<List<Order>> GetAll();
		Task<int> CreateOrder(Order user);
		Task<int> UpdateOrderAsync(Order user);
        Task<Order?> GetOrderById(long id);

        Task<string> SearchOrder(long id, string email);
    }
}
