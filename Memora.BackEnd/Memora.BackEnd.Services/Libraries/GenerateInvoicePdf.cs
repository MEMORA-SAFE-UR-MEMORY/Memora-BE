using Memora.BackEnd.Repositories.Models;
using Microsoft.Extensions.Logging;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
using System.IO;

namespace Memora.BackEnd.Services.Libraries
{
	public static class GenerateInvoicePdf
	{
		private static bool _fontsRegistered = false;
		private static readonly object _lock = new object();

		private static void RegisterFonts(ILogger logger)
		{
			lock (_lock)
			{
				if (_fontsRegistered)
					return;

				try
				{
					var fontDirectory = Path.Combine(AppContext.BaseDirectory, "LatoFont");
					if (Directory.Exists(fontDirectory))
					{
						FontManager.RegisterFont(File.OpenRead(Path.Combine(fontDirectory, "Lato-Regular.ttf")));
						FontManager.RegisterFont(File.OpenRead(Path.Combine(fontDirectory, "Lato-Bold.ttf")));
						logger.LogInformation("Successfully registered Lato fonts for QuestPDF.");
					}
					else
					{
						logger.LogWarning("LatoFont directory not found at {FontDirectory}. PDF generation may fallback to default system fonts.", fontDirectory);
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed to register custom fonts for QuestPDF.");
				}
				finally
				{
					_fontsRegistered = true;
				}
			}
		}

		public static string GenerateInvoice(Order order, User user, byte[]? logoBytes, ILogger logger)
		{
			RegisterFonts(logger);

			var fileName = $"Invoice_{order.PayOsOrderCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
			var filePath = Path.Combine(Path.GetTempPath(), fileName);

			QuestPDF.Settings.DocumentLayoutExceptionThreshold = 10000;

			Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(40);
					page.Size(PageSizes.A5);
					page.PageColor("#FAF6FF");
					page.DefaultTextStyle(x => x.FontSize(10).FontColor("#4B4B8F").FontFamily("Lato"));

					page.Header()
						.Column(column =>
						{
							column.Spacing(5);

							if (logoBytes != null && logoBytes.Length > 0)
							{
								try
								{
									column.Item().AlignCenter().Height(50).Image(logoBytes);
								}
								catch (Exception ex)
								{
									logger.LogWarning(ex, "Failed to render logo image in PDF. The image data might be corrupt.");
								}
							}

							column.Item().AlignCenter().Text("Memora – Triển lãm ký ức")
								.FontSize(20)
								.Bold()
								.FontColor("#C58AC9");

							column.Item().AlignCenter().Text("HÓA ĐƠN THANH TOÁN")
								.FontSize(14)
								.SemiBold()
								.FontColor("#6CA6E0");
						});

					page.Content()
						.PaddingVertical(20)
						.Column(col =>
						{
							col.Spacing(15);

							col.Item().Row(row =>
							{
								row.RelativeItem().Column(c =>
								{
									c.Spacing(2);
									c.Item().Text("Khách hàng").SemiBold().FontColor(Colors.Grey.Medium);
									c.Item().Text(user.Fullname ?? user.Username).Bold();
									c.Item().Text(user.Email);
								});

								row.RelativeItem().Column(c =>
								{
									c.Spacing(2);
									c.Item().AlignRight().Text("Mã đơn hàng").SemiBold().FontColor(Colors.Grey.Medium);
									c.Item().AlignRight().Text($"#{order.PayOsOrderCode}").Bold();
								});
							});

							col.Item().LineHorizontal(1).LineColor("#D9D9FF");

							col.Item().Table(table =>
							{
								table.ColumnsDefinition(columns =>
								{
									columns.RelativeColumn(3);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
								});

								table.Header(header =>
								{
									header.Cell().Text("Sản phẩm").Bold();
									header.Cell().AlignRight().Text("Số lượng").Bold();
									header.Cell().AlignRight().Text("Đơn giá").Bold();
									header.Cell().AlignRight().Text("Thành tiền").Bold();
								});

								foreach (var item in order.OrderAlbums)
								{
									table.Cell().Text(item.Album.Name).WrapAnywhere();
									table.Cell().AlignRight().Text(item.Quantity.ToString());
									table.Cell().AlignRight().Text($"{item.Price:N0} VND");
									table.Cell().AlignRight().Text($"{(item.Quantity * item.Price):N0} VND");
								}

								table.Cell().ColumnSpan(4).PaddingTop(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

								table.Cell().ColumnSpan(3).AlignRight().Text("Tổng cộng").Bold();
								table.Cell().AlignRight().Text($"{order.TotalPrice:N0} VND").Bold();
							});

							col.Item().PaddingTop(10).Column(c => {
								c.Spacing(2);
								c.Item().Text("Ghi chú giao dịch").SemiBold().FontColor(Colors.Grey.Medium);
								c.Item().Text($"Thanh toán cho đơn hàng #{order.PayOsOrderCode}");
								c.Item().Text($"Ngày thanh toán: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}");
								c.Item().Text("Trạng thái: Đã thanh toán thành công").FontColor("#4CAF50").Bold();
							});
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