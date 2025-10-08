using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Memora.BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _orderService.GetAllAsync();

            if (orders == null || !orders.Any())
            {
                return Ok(new { message = "Hiện không có order nào :(" });
            }

            return Ok(orders);
        }

        [HttpGet("getById/{id:long}")]
        public async Task<IActionResult> GetOrderById([FromRoute]long id)
        {
            var order = await _orderService.GetOrderById(id);

            if (order == null)
            {
                return Ok(new { message = "Không tìm thấy order :(" });
            }

            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CreateOrderAsync(request);

            if (result == 0)
                return BadRequest(new { message = "UserId không hợp lệ!" });

            if (result > 0)
                return Ok(new { message = "Tạo order thành công!" });

            return StatusCode(500, new { message = "Tạo order thất bại!" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
        {
            var result = await _orderService.UpdateOrderAsync(request);

            if (result > 0)
                return Ok(new { message = "Cập nhật order thành công!" });

            return StatusCode(500, new { message = "Cập nhật order thất bại!" });
        }
    }
}
