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
            return await _context.Orders.Include(a => a.OrderAlbums).Include(a => a.User).ToListAsync();
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
    }
}
