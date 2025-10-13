using System.Text.Json.Serialization;

namespace Memora.BackEnd.Services.Dtos
{
	public class PayOsWebhookData
	{
		[JsonPropertyName("orderCode")]
		public long OrderCode { get; set; }

		[JsonPropertyName("amount")]
		public int Amount { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; } = string.Empty;

		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;

		// Thêm các trường khác nếu cần
	}
}
