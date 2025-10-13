using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace Memora.BackEnd.Services.Services
{
	public class SupabaseFileService : ISupabaseFileService
	{
		private readonly Client _supabaseClient;

		public SupabaseFileService(IConfiguration configuration)
		{
			var supabaseUrl = configuration["Supabase:Url"];
			var supabaseKey = configuration["Supabase:Key"];

			if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
			{
				throw new ArgumentException("Supabase URL and Key must be configured in appsettings.json.");
			}

			var options = new SupabaseOptions
			{
				AutoRefreshToken = true,
				AutoConnectRealtime = true
			};

			_supabaseClient = new Client(supabaseUrl, supabaseKey, options);
		}

		public async Task<string> UploadFileAsync(IFormFile file, string bucketName, string? folder = null)
		{
			try
			{
				if (file == null || file.Length == 0)
				{
					throw new ArgumentException("File is null or empty.");
				}

				var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
				var filePath = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);
				var fileBytes = memoryStream.ToArray();

				var storage = _supabaseClient.Storage.From(bucketName);
				var response = await storage.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions { CacheControl = "3600", Upsert = false });

				if (string.IsNullOrEmpty(response))
				{
					throw new Exception("File upload to Supabase failed.");
				}

				var publicUrl = storage.GetPublicUrl(filePath);

				return publicUrl;
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Error uploading file to Supabase: {ex.Message}");
			}
		}

		public async Task<string> UploadFileSaveVersionAsync(IFormFile file, string bucketName, string? folder = null)
		{
			try
			{
				if (file == null || file.Length == 0)
					throw new ArgumentException("File is null or empty.");

				var storage = _supabaseClient.Storage.From(bucketName);

				if (!string.IsNullOrEmpty(folder))
				{
					var list = await storage.List(folder);
					if (list != null && list.Count > 0)
					{
						var oldFiles = list.Select(f => $"{folder}/{f.Name}").ToList();
						await storage.Remove(oldFiles);
					}
				}

				var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
				var filePath = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);
				var fileBytes = memoryStream.ToArray();

				var response = await storage.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions
				{
					CacheControl = "3600",
					Upsert = false
				});

				if (string.IsNullOrEmpty(response))
					throw new Exception("File upload to Supabase failed.");

				var publicUrl = storage.GetPublicUrl(filePath);
				return publicUrl;
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Error uploading file to Supabase: {ex.Message}");
			}
		}
	}
}
