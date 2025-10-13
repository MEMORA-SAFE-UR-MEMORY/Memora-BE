namespace Memora.BackEnd.Services.Dtos
{
	public class PaymentLinkDto
	{
		public string? Code { get; set; }
		public string? Desc { get; set; }
		public PaymentLinkDataDto? Data { get; set; }
		public string? Signature { get; set; }
	}
}
