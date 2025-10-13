using Memora.BackEnd.Services.Dtos;
using System.Text.Json;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface IPayOsService
	{
		Task<PaymentLinkDto?> CreatePaymentLink(long orderCode, int amount, string description, string cancelUrl, string returnUrl);
		bool VerifySignature(JsonElement? data, string expectedSignature);
	}
}
