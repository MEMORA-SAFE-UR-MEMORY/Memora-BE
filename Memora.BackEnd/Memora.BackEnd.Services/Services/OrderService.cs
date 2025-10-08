using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Libraries; // thêm dòng này để dùng EmailService

namespace Memora.BackEnd.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository; // cần để lấy thông tin user
        private readonly EmailService _email; // thêm EmailService

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, EmailService email)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _email = email;
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
                PhoneNumber = o.PhoneNumber,
                Fullname = o.Fullname,
                Address = o.Address,
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
            //check userid
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) return 0;

            var order = new Order
            {
                Status = request.Status,
                TotalPrice = request.TotalPrice,
                UserId = request.UserId,
                Address = request.Address,
                Fullname= request.Fullname,
                PhoneNumber= request.PhoneNumber,
                OrderAlbums = request.OrderAlbums.Select(oa => new OrderAlbum
                {
                    AlbumId = oa.AlbumId,
                    Quantity = oa.Quantity,
                    Price = oa.Price
                }).ToList()
            };

            var result = await _orderRepository.CreateOrder(order);

            // ✅ Sau khi tạo order thành công, gửi mail cho user
            if (result > 0)
            {
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = "Xác nhận đơn hàng của bạn";
                    string message = $@"
Xin chào {user.Fullname ?? user.Username},
Cảm ơn bạn đã đặt hàng tại Memora! 🎉
Thông tin đơn hàng:
-------------------------------
Mã đơn hàng: #{order.Id}
Ngày đặt: {DateTime.UtcNow:dd/MM/yyyy HH:mm}
Tổng tiền: {order.TotalPrice:N0} VND
Trạng thái: {order.Status}
Thông tin người nhận:
-------------------------------
Họ tên: {order.Fullname}
Số điện thoại: {order.PhoneNumber}
Địa chỉ: {order.Address}
-------------------------------
Vui lòng truy cập website Memora để tiến hành thanh toán đơn hàng của bạn.
Trân trọng,
Đội ngũ Memora
";

                    await _email.SendEmailAsync(user.Email, subject, message);
                }
            }

            return result;
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

            return new OrderDto
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
                PhoneNumber = o.PhoneNumber,
                Fullname = o.Fullname,
                Address = o.Address,
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
