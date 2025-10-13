namespace Memora.BackEnd.Services.Dtos
{
	public class PaymentLinkDataDto
	{
		public string? Bin { get; set; }
		public string? AccountNumber { get; set; }
		public string? AccountName { get; set; }
		public int Amount { get; set; }
		public string? Description { get; set; }
		public long OrderCode { get; set; }
		public string? PaymentLinkId { get; set; }
		public string? Status { get; set; }
		public string? CheckoutUrl { get; set; }
		public string? QrCode { get; set; }
	}
}
