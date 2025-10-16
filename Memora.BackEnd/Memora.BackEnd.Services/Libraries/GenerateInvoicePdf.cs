using Memora.BackEnd.Repositories.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Memora.BackEnd.Services.Libraries
{
	public static class GenerateInvoicePdf
	{
		public static string GenerateInvoicePdfAsync(Order order, User user, string logoPath)
		{
			var fileName = $"Invoice_{order.Id}.pdf";
			var filePath = Path.Combine(Path.GetTempPath(), fileName);
			var today = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

			Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(40);
					page.Size(PageSizes.A5);
					page.PageColor("#FAF6FF"); 
					page.DefaultTextStyle(x => x.FontSize(12).FontColor("#4B4B8F")); 

					page.Header()
						.Column(column =>
						{
							column.Spacing(6);

							if (File.Exists(logoPath))
								column.Item().AlignCenter().Height(50).Image(Image.FromFile(logoPath));

							column.Item().AlignCenter().Text("Memora – Triển lãm ký ức")
								.FontSize(18)
								.Bold()
								.FontColor("#C58AC9"); 

							column.Item().AlignCenter().Text("HÓA ĐƠN THANH TOÁN ALBUM")
								.FontSize(14)
								.Bold()
								.FontColor("#6CA6E0"); 
						});

					page.Content()
						.PaddingVertical(20)
						.Column(col =>
						{
							col.Spacing(10);

							col.Item().Text($"📦 Mã đơn hàng: #{order.Id}")
								.FontColor("#4B4B8F");
							col.Item().Text($"👤 Khách hàng: {user.Fullname ?? user.Username}")
								.FontColor("#4B4B8F");
							col.Item().Text($"🕒 Ngày thanh toán: {today}")
								.FontColor("#4B4B8F");
							col.Item().Text($"💳 Mã giao dịch: {order.PayOsOrderCode}")
								.FontColor("#4B4B8F");
							col.Item().Text($"💰 Tổng tiền: {order.TotalPrice.ToString("N0")} VND")
								.Bold()
								.FontColor("#C58AC9");
							col.Item().Text($"✅ Trạng thái: ĐÃ THANH TOÁN")
								.Bold()
								.FontColor("#4CAF50");
						});

					page.Footer()
						.AlignCenter()
						.Column(footer =>
						{
							footer.Item().Text("Cảm ơn bạn đã chọn Memora 💜")
								.FontSize(10)
								.FontColor("#C58AC9");

							footer.Item().Text("Email: memora940@gmail.com | Hotline: 0559 670 539")
								.FontSize(9)
								.FontColor("#6CA6E0");
						});
				});
			})
			.GeneratePdf(filePath);

			return filePath;
		}
	}
}
