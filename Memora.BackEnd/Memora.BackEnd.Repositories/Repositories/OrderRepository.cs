using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PostgresContext _context;

        public OrderRepository(PostgresContext context) => _context = context;

        public async Task<int> CreateOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            // chỉ add order + orderAlbums, không set Id/CreatedAt/UpdatedAt
            await _context.Orders.AddAsync(order);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderAlbums)
                    .ThenInclude(oa => oa.Album)
                        .ThenInclude(a => a.Template)
                .ToListAsync();
        }


        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _context.Orders.Include(o => o.User)
                .Include(o => o.OrderAlbums)
                    .ThenInclude(oa => oa.Album)
                        .ThenInclude(a => a.Template).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<string> SearchOrder(Guid id, string email)
        {
            var order = await _context.Orders.Include(o => o.User)
                .Include(o => o.OrderAlbums)
                    .ThenInclude(oa => oa.Album)
                        .ThenInclude(a => a.Template).FirstOrDefaultAsync(a => a.Id == id && a.User.Email == email);

            if (order == null) return null;

            return order.Status;
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
                throw new KeyNotFoundException($"Order with id {order.Id} not found");

            // chỉ update status
            existingOrder.Status = order.Status;

            // không đụng tới TotalPrice, UserId, OrderAlbums
            _context.Orders.Update(existingOrder);

            return await _context.SaveChangesAsync();
        }

		public async Task<Order?> GetByPayOsOrderCodeAsync(long orderCode)
		{
			return await _context.Orders.FirstOrDefaultAsync(o => o.PayOsOrderCode == orderCode);
		}

		public async Task<int> UpdateStatusByPayOsOrderCodeAsync(long orderCode, string status)
		{
			var order = await _context.Orders.AsTracking().FirstOrDefaultAsync(o => o.PayOsOrderCode == orderCode);
			if (order == null)
			{
				return 0;
			}
			order.Status = status;
			order.UpdatedAt = DateTime.UtcNow;
			return await _context.SaveChangesAsync();
		}

		public async Task<int> SetPayOsOrderCodeAsync(Guid orderId, long orderCode)
		{
			var order = await _context.Orders.AsTracking().FirstOrDefaultAsync(o => o.Id == orderId);
			if (order == null)
			{
				return 0;
			}
			order.PayOsOrderCode = orderCode;
			return await _context.SaveChangesAsync();
		}
	}
}
