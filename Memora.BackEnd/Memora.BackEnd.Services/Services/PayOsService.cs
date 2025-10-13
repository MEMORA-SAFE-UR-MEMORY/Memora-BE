using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Memora.BackEnd.Services.Services
{
	public class PayOsService : IPayOsService
	{
		private readonly HttpClient _httpClient;
		private readonly PayOSSettings _payOsSettings;
		private readonly ILogger<PayOsService> _logger;

		public PayOsService(HttpClient httpClient, IOptions<PayOSSettings> payOsSettings, ILogger<PayOsService> logger)
		{
			_httpClient = httpClient;
			_payOsSettings = payOsSettings.Value;
			_logger = logger;
		}

		public async Task<PaymentLinkDto?> CreatePaymentLink(long orderCode, int amount, string description, string cancelUrl, string returnUrl)
		{
			if (string.IsNullOrEmpty(_payOsSettings.ChecksumKey))
			{
				_logger.LogError("CRITICAL ERROR: PayOS ChecksumKey is not loaded from configuration. Please check Program.cs for services.Configure<PayOSSettings>().");
				// Trả về lỗi để lập trình viên biết ngay vấn đề
				return new PaymentLinkDto { Code = "CONFIG_ERROR", Desc = "PayOS ChecksumKey is missing." };
			}

			// Sử dụng SortedDictionary để đảm bảo các key được sắp xếp theo thứ tự alphabet
			var signatureData = new SortedDictionary<string, string>
			{
				{ "amount", amount.ToString() },
				{ "cancelUrl", cancelUrl },
				{ "description", description },
				{ "orderCode", orderCode.ToString() },
				{ "returnUrl", returnUrl }
			};

			string dataToSign = string.Join("&", signatureData.Select(kvp => $"{kvp.Key}={kvp.Value}"));

			_logger.LogInformation("Final data string to be signed: \"{dataToSign}\"", dataToSign);

			var signature = CreateSignatureFromString(dataToSign);
			_logger.LogInformation("Generated Signature: {signature}", signature);

			var payload = new
			{
				orderCode,
				amount,
				description,
				cancelUrl,
				returnUrl,
				signature
			};

			var jsonPayload = JsonSerializer.Serialize(payload);
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			try
			{
				var response = await _httpClient.PostAsync("/v2/payment-requests", content);
				var responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					return JsonSerializer.Deserialize<PaymentLinkDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				}
				else
				{
					_logger.LogError("PayOS API call failed. Status: {StatusCode}, Body: {ErrorBody}", response.StatusCode, responseBody);
					return JsonSerializer.Deserialize<PaymentLinkDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occurred during PayOS API call.");
				return null;
			}
		}

		private string CreateSignatureFromString(string dataToSign)
		{
			using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_payOsSettings.ChecksumKey));
			var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}

		public string CreateSignature(object data)
		{
			var dataJson = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
			var dataDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dataJson);

			var sortedDict = new SortedDictionary<string, string>();
			if (dataDict != null)
			{
				foreach (var item in dataDict)
				{
					string valueStr = (item.Value.ValueKind == JsonValueKind.Number)
										? item.Value.GetInt64().ToString()
										: item.Value.GetString() ?? "";
					sortedDict.Add(item.Key, valueStr);
				}
			}

			var dataToSign = string.Join("&", sortedDict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
			return CreateSignatureFromString(dataToSign);
		}

		public bool VerifySignature(JsonElement? data, string expectedSignature)
		{
			if (data == null) return false;

			var rawJson = data.Value.GetRawText();
			var dataObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(rawJson);

			var sortedDict = new SortedDictionary<string, string>();
			if (dataObj != null)
			{
				foreach (var item in dataObj)
				{
					string valueStr = (item.Value.ValueKind == JsonValueKind.Number)
						? item.Value.GetInt64().ToString()
						: item.Value.GetString() ?? "";

					sortedDict.Add(item.Key, valueStr);
				}
			}

			var dataToSign = string.Join("&", sortedDict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
			var computedSignature = CreateSignatureFromString(dataToSign);

			_logger.LogInformation("Webhook Data to sign: {DataToSign}", dataToSign);
			_logger.LogInformation("Computed Webhook Signature: {ComputedSignature}", computedSignature);
			_logger.LogInformation("Expected Webhook Signature: {ExpectedSignature}", expectedSignature);

			return computedSignature == expectedSignature;
		}
	}
}