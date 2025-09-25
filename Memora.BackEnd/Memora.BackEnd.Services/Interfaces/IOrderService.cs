using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<int> UpdateOrderAsync(UpdateOrderRequest request);
        Task<OrderDto?> GetOrderById(long id);
    }
}
