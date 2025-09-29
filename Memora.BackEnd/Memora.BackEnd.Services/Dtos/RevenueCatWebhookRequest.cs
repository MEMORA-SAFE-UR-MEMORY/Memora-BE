using System.Text.Json.Serialization;

namespace Memora.BackEnd.Services.Dtos
{
	public class RevenueCatWebhookRequest
	{
		[JsonPropertyName("api_version")]
		public string ApiVersion { get; set; } = string.Empty;

		[JsonPropertyName("event")]
		public RevenueCatEvent Event { get; set; } = null!;
	}
}
