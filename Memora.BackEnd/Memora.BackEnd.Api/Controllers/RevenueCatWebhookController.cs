// Memora.BackEnd/Memora.BackEnd.Api/Controllers/RevenueCatWebhookController.cs
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Memora.BackEnd.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RevenueCatWebhookController : ControllerBase
	{
		private readonly IPurchaseService _purchaseService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<RevenueCatWebhookController> _logger;

		public RevenueCatWebhookController(IPurchaseService purchaseService, IConfiguration configuration, ILogger<RevenueCatWebhookController> logger)
		{
			_purchaseService = purchaseService;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost("revenuecat")]
		public async Task<IActionResult> HandleRevenueCatWebhook()
		{
			var signatureHeader = Request.Headers["Authorization"].ToString();
			if (string.IsNullOrEmpty(signatureHeader))
			{
				_logger.LogWarning("RevenueCat Webhook: Missing Authorization header.");
				return Unauthorized("Authorization header is missing.");
			}

			using var reader = new StreamReader(Request.Body, Encoding.UTF8);
			var requestBody = await reader.ReadToEndAsync();

			var secret = _configuration["RevenueCatSettings:WebhookSecretKey"];
			if (string.IsNullOrEmpty(secret))
			{
				_logger.LogError("RevenueCatSettings:WebhookSecretKey is not configured.");
				return StatusCode(500, "Webhook secret is not configured.");
			}

			if (!IsSignatureValid(signatureHeader, requestBody, secret))
			{
				_logger.LogWarning("RevenueCat Webhook: Invalid signature.");
				return Unauthorized("Invalid signature.");
			}

			try
			{
				var webhookPayload = JsonSerializer.Deserialize<RevenueCatWebhookRequest>(requestBody);
				if (webhookPayload?.Event == null)
				{
					return BadRequest("Invalid webhook payload.");
				}

				var eventType = webhookPayload.Event.Type;
				_logger.LogInformation("Received RevenueCat webhook of type: {EventType}", eventType);

				if (eventType is "INITIAL_PURCHASE" or "NON_RENEWING_PURCHASE")
				{
					await _purchaseService.GrantThemeAccessAsync(
						webhookPayload.Event.AppUserId,
						webhookPayload.Event.ProductId);
				}

				return Ok();
			}
			catch (JsonException jsonEx)
			{
				_logger.LogError(jsonEx, "Error deserializing RevenueCat webhook payload.");
				return BadRequest("Invalid JSON payload.");
			}
			catch (KeyNotFoundException knfEx)
			{
				_logger.LogWarning(knfEx.Message);
				return BadRequest(knfEx.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred while processing RevenueCat webhook.");
				return StatusCode(500, "An internal error occurred.");
			}
		}

		private static bool IsSignatureValid(string header, string payload, string secret)
		{
			try
			{
				var keyValues = header.Replace("RC-Signature ", "").Split(',');
				var signatureInfo = keyValues.ToDictionary(
					kv => kv.Split('=')[0],
					kv => kv.Split('=')[1]
				);

				var providedTimestamp = signatureInfo["t"];
				var providedSignature = signatureInfo["v1"];

				var dataToSign = $"{providedTimestamp}.{payload}";

				using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
				var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

				return computedSignature.Equals(providedSignature, StringComparison.Ordinal);
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}