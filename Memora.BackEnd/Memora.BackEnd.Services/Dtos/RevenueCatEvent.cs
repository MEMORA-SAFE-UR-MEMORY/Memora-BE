using System.Text.Json.Serialization;

namespace Memora.BackEnd.Services.Dtos
{
	public class RevenueCatEvent
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;

		[JsonPropertyName("app_user_id")]
		public string AppUserId { get; set; } = string.Empty;

		[JsonPropertyName("product_id")]
		public string ProductId { get; set; } = string.Empty;

		[JsonPropertyName("store")]
		public string Store { get; set; } = string.Empty;
	}
}
