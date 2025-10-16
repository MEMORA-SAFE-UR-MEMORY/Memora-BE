using System.Text.Json;
using System.Text.Json.Serialization;

namespace Memora.BackEnd.Services.Dtos
{
	public class PayOsWebhookRequest
	{
		[JsonPropertyName("code")]
		public string Code { get; set; } = string.Empty;

		[JsonPropertyName("desc")]
		public string Desc { get; set; } = string.Empty;

		[JsonPropertyName("data")]
		public JsonElement Data { get; set; }

		[JsonPropertyName("signature")]
		public string Signature { get; set; } = string.Empty;
	}
}
