using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Libraries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Globalization;
using System.Text;

namespace Memora.BackEnd.Services.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IUserRepository _userRepository;
		private readonly EmailService _email;
		private readonly IConfiguration _configuration;
		private readonly IPayOsService _payOsService;
		private readonly ILogger<OrderService> _logger;

		public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, EmailService email, IConfiguration configuration, IPayOsService payOsService, ILogger<OrderService> logger)
		{
			_orderRepository = orderRepository;
			_userRepository = userRepository;
			_email = email;
			_configuration = configuration;
			_payOsService = payOsService;
			_logger = logger;
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
			var user = await _userRepository.GetByIdAsync(request.UserId);
			if (user == null) return 0;

			var order = new Order
			{
				Status = request.Status,
				TotalPrice = request.TotalPrice,
				UserId = request.UserId,
				Address = request.Address,
				Fullname = request.Fullname,
				PhoneNumber = request.PhoneNumber,
				OrderAlbums = request.OrderAlbums.Select(oa => new OrderAlbum
				{
					AlbumId = oa.AlbumId,
					Quantity = oa.Quantity,
					Price = oa.Price
				}).ToList()
			};

			var result = await _orderRepository.CreateOrder(order);

			if (result > 0)
			{
				if (user != null && !string.IsNullOrEmpty(user.Email))
				{
					string subject = "Xác nhận đơn hàng của bạn";
					string message = $@"
Xin chào {user.Fullname ?? user.Username},
Cảm ơn bạn đã đặt hàng tại Memora! 🎉
-------------------------------
Thông tin đơn hàng:
Mã đơn hàng: #{order.Id}
Ngày đặt: {DateTime.UtcNow:dd/MM/yyyy HH:mm}
Tổng tiền: {order.TotalPrice:N0} VND
Trạng thái: {order.Status}
-------------------------------
Thông tin người nhận:
Họ tên: {order.Fullname}
Số điện thoại: {order.PhoneNumber}
Địa chỉ: {order.Address}
-------------------------------
Vui lòng truy cập website Memora, login với tài khoản của bạn để tiến hành thanh toán và theo dõi đơn hàng:
👉 https://memora-official.com/

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

		public async Task<OrderDto?> GetOrderById(Guid id)
		{
			var o = await _orderRepository.GetOrderById(id);
			if (o == null)
			{
				return null;
			}

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

		public Task<string> SearchOrder(Guid id, string email)
		{
			return _orderRepository.SearchOrder(id, email);
		}

		public async Task<PaymentLinkDto?> CreatePaymentLinkForOrderAsync(Guid orderId)
		{
			var order = await _orderRepository.GetOrderById(orderId);
			if (order == null)
			{
				return null;
			}

			var payOsOrderCode = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
			await _orderRepository.SetPayOsOrderCodeAsync(orderId, payOsOrderCode);

			var amount = (int)order.TotalPrice;
			var description = $"Memora ThanhToanAlbum_{order.Id.ToString()[..4]}";

			var clientBaseUrl = _configuration["App:ClientUrl"]
				?? throw new InvalidOperationException("App:ClientUrl is not configured in appsettings.json");
			var returnUrl = $"{clientBaseUrl}/success";
			var cancelUrl = $"{clientBaseUrl}/failed";

			return await _payOsService.CreatePaymentLink(payOsOrderCode, amount, description, cancelUrl, returnUrl);
		}

		public async Task<int> UpdateOrderStatusFromWebhookAsync(long orderCode, string code)
		{
			var order = await _orderRepository.GetByPayOsOrderCodeAsync(orderCode);
			if (order == null) return 0;

			if (code == "00") 
			{
				if (order.Status == "Đã thanh toán") return 1; 

				var result = await _orderRepository.UpdateStatusByPayOsOrderCodeAsync(orderCode, "Đã thanh toán");

				if (result > 0)
				{
					var fullOrder = await _orderRepository.GetOrderById(order.Id);
					if (fullOrder == null)
					{
						_logger.LogError("Could not find order with ID {OrderId} after updating status.", order.Id);
						return 0;
					}

					var user = fullOrder.User;
					if (user != null && !string.IsNullOrEmpty(user.Email))
					{
						try
						{
							string subject = $"Hóa đơn thanh toán thành công – Đơn hàng #{fullOrder.PayOsOrderCode}";
							var bodyBuilder = new BodyBuilder();

							var htmlBody = new StringBuilder();
							htmlBody.Append(@"
<div style='font-family: Arial, sans-serif; color: #333; line-height: 1.6; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px; background-color: #FAF6FF;'>
    <div style='text-align: center; margin-bottom: 20px;'>
        <img src='https://yzzispiaqactvbvsjwcw.supabase.co/storage/v1/object/public/System/memora.png' alt='Memora Logo' style='height: 50px;'>
        <h1 style='color: #C58AC9; font-size: 24px; margin-top: 10px;'>Memora – Triển lãm ký ức</h1>
        <h2 style='color: #6CA6E0; font-size: 18px; font-weight: normal;'>HÓA ĐƠN THANH TOÁN</h2>
    </div>
    <p>Chào <strong>");
							htmlBody.Append(user.Fullname ?? user.Username);
							htmlBody.Append(@"</strong>,</p>
    <p>Cảm ơn bạn đã mua sắm tại Memora. Đơn hàng của bạn đã được thanh toán thành công!</p>
    
    <div style='margin-top: 25px; margin-bottom: 25px; padding: 15px; border: 1px solid #D9D9FF; border-radius: 5px; background-color: #fff;'>
        <h3 style='margin-top: 0; color: #4B4B8F;'>Chi tiết đơn hàng</h3>
        <p><strong>Mã đơn hàng:</strong> #");
							htmlBody.Append(fullOrder.PayOsOrderCode);
							htmlBody.Append(@"</p>
        <p><strong>Ngày thanh toán:</strong> ");
							htmlBody.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
							htmlBody.Append(@"</p>
        <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
            <thead style='background-color: #f2e9ff;'>
                <tr>
                    <th style='padding: 8px; border: 1px solid #D9D9FF; text-align: left;'>Sản phẩm</th>
                    <th style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>Số lượng</th>
                    <th style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>Đơn giá</th>
                    <th style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>Thành tiền</th>
                </tr>
            </thead>
            <tbody>");

							foreach (var item in fullOrder.OrderAlbums)
							{
								htmlBody.Append($@"
                <tr>
                    <td style='padding: 8px; border: 1px solid #D9D9FF;'>{item.Album.Name}</td>
                    <td style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>{item.Quantity}</td>
                    <td style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>{item.Price:N0} VND</td>
                    <td style='padding: 8px; border: 1px solid #D9D9FF; text-align: right;'>{(item.Price * item.Quantity):N0} VND</td>
                </tr>");
							}

							htmlBody.Append($@"
            </tbody>
            <tfoot>
                <tr>
                    <td colspan='3' style='padding: 8px; text-align: right; font-weight: bold;'>Tổng cộng</td>
                    <td style='padding: 8px; border: 1px solid #D9D9FF; text-align: right; font-weight: bold;'>{fullOrder.TotalPrice:N0} VND</td>
                </tr>
            </tfoot>
        </table>
    </div>
    
    <p>Đơn hàng của bạn sẽ sớm được xử lý và vận chuyển.</p>
    <p>Trân trọng,<br><strong>Đội ngũ Memora</strong></p>
    <hr style='border: none; border-top: 1px solid #eee; margin-top: 20px;' />
    <div style='text-align: center; font-size: 0.8em; color: #777;'>
        <p>Hotline: 0559 670 539 | Email: memora940@gmail.com | Website: <a href='https://memora-official.com' style='color: #6CA6E0;'>memora-official.com</a></p>
    </div>
</div>
");
							bodyBuilder.HtmlBody = htmlBody.ToString();

							await _email.SendEmailAsync(user.Email, subject, bodyBuilder);
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Failed to build or send invoice email for order {OrderId}.", fullOrder.Id);
						}
					}
				}
				return result;
			}
			return 1; 
		}
	}
}