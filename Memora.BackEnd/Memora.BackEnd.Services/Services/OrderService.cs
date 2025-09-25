using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;

namespace Memora.BackEnd.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAll();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                UserInfo = new UserOrderDto
                {
                    Id = o.UserId,
                    Username = o.User.Username,
                    Fullname = o.User.Fullname,
                    Address = o.User.Address,
                    PhoneNumber = o.User.PhoneNumber
                },
                OrderAlbums = o.OrderAlbums.Select(oa => new OrderAlbumDto
                {
                    Id = oa.Id,
                    AlbumDto = new AlbumDto
                    {
                        Id = oa.Album.Id,
                        Name = oa.Album.Name,
                        Template = new AlbumTemplateDto
                        {
                            Id = oa.Album.Template.Id,
                            Name = oa.Album.Template.Name
                        }
                    },
                    Price = oa.Price,
                    Quantity = oa.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            var order = new Order
            {
                Status = request.Status,
                TotalPrice = request.TotalPrice,
                UserId = request.UserId,
                OrderAlbums = request.OrderAlbums.Select(oa => new OrderAlbum
                {
                    AlbumId = oa.AlbumId,
                    Quantity = oa.Quantity,
                    Price = oa.Price
                }).ToList()
            };

            return await _orderRepository.CreateOrder(order);
        }

        public async Task<int> UpdateOrderAsync(UpdateOrderRequest request)
        {
            var order = new Order
            {
                Id = request.Id,
                Status = request.Status
            };

            return await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task<OrderDto?> GetOrderById(long id)
        {
            var o = await _orderRepository.GetOrderById(id);

            return new OrderDto {
                Id = o.Id,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                UserInfo = new UserOrderDto
                {
                    Id = o.UserId,
                    Username = o.User.Username,
                    Fullname = o.User.Fullname,
                    Address = o.User.Address,
                    PhoneNumber = o.User.PhoneNumber
                },
                OrderAlbums = o.OrderAlbums.Select(oa => new OrderAlbumDto
                {
                    Id = oa.Id,
                    AlbumDto = new AlbumDto
                    {
                        Id = oa.Album.Id,
                        Name = oa.Album.Name,
                        Template = new AlbumTemplateDto
                        {
                            Id = oa.Album.Template.Id,
                            Name = oa.Album.Template.Name
                        }
                    },
                    Price = oa.Price,
                    Quantity = oa.Quantity
                }).ToList(),
            };
        }
    }
}
