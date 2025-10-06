using Microsoft.AspNetCore.Http;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface ISupabaseFileService
	{
		Task<string> UploadFileAsync(IFormFile file, string bucketName, string? folder = null);
	}
}
