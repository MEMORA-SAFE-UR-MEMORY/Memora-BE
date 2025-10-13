using Microsoft.AspNetCore.Http;

namespace Memora.BackEnd.Services.Dtos
{
	public class ImageRequest
	{
		public long Id { get; set; }
		public IFormFile Photo { get; set; } = null!;
	}
}
