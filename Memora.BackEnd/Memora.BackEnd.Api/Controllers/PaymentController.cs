using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Memora.BackEnd.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly IPayOsService _payOsService;
		private readonly ILogger<PaymentController> _logger;

		public PaymentController(IOrderService orderService, IPayOsService payOsService, ILogger<PaymentController> logger)
		{
			_orderService = orderService;
			_payOsService = payOsService;
			_logger = logger;
		}

		[HttpPost("create-link")]
		public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequestDto request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var paymentLinkInfo = await _orderService.CreatePaymentLinkForOrderAsync(request.OrderId);

			if (paymentLinkInfo == null)
			{
				return BadRequest(new { message = "Không thể tạo link thanh toán. Vui lòng kiểm tra lại thông tin đơn hàng." });
			}

			return Ok(paymentLinkInfo);
		}

		//[AllowAnonymous]
		//[HttpGet("return")]
		//public IActionResult PaymentReturn([FromQuery] string code, [FromQuery] string id, [FromQuery] bool cancel, [FromQuery] string status, [FromQuery] long orderCode)
		//{
		//	return Ok(new { code, id, cancel, status, orderCode });
		//}

		[AllowAnonymous]
		[HttpPost("webhook")]
		public async Task<IActionResult> HandlePayOsWebhook([FromBody] PayOsWebhookRequest webhookRequest)
		{
			_logger.LogInformation("Received PayOS webhook: {WebhookBody}", JsonSerializer.Serialize(webhookRequest));

			if (webhookRequest == null || string.IsNullOrEmpty(webhookRequest.Signature))
			{
				_logger.LogWarning("Invalid webhook request.");
				return BadRequest(new { message = "Invalid request" });
			}

			if (!_payOsService.VerifySignature(webhookRequest.Data, webhookRequest.Signature))
			{
				_logger.LogWarning("Webhook signature verification failed.");
				return BadRequest(new { message = "Invalid signature" });
			}

			try
			{
				var webhookData = JsonSerializer.Deserialize<PayOsWebhookData>(webhookRequest.Data.GetRawText() ?? "{}");
				if (webhookData == null)
				{
					_logger.LogWarning("Failed to deserialize webhook data.");
					return BadRequest(new { message = "Invalid data" });
				}

				_logger.LogInformation($"Webhook signature verified. Processing order code: {webhookData.OrderCode}");
				await _orderService.UpdateOrderStatusFromWebhookAsync(webhookData.OrderCode, webhookData.Code);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing PayOS webhook.");
				return StatusCode(500, new { message = "Internal server error" });
			}
		}
	}
}
