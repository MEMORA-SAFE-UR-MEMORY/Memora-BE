using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<int> UpdateOrderAsync(UpdateOrderRequest request);
        Task<OrderDto?> GetOrderById(Guid id);

        Task<string> SearchOrder(Guid id, string email);

		Task<PaymentLinkDto?> CreatePaymentLinkForOrderAsync(Guid orderId);
		Task<int> UpdateOrderStatusFromWebhookAsync(long orderCode, string status);
	}
}
