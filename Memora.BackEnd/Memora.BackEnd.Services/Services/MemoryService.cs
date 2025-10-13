using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;

namespace Memora.BackEnd.Services.Services
{
	public class MemoryService : IMemoryService
	{
		private readonly IMemoryRepository _repo;
		private readonly ISupabaseFileService _supabaseFileService;
		public MemoryService(IMemoryRepository repo, ISupabaseFileService supabaseFileService)
		{
			_repo = repo;
			_supabaseFileService = supabaseFileService;
		}
		public async Task<int> UpdateAsync(ImageRequest dto)
		{
			try
			{
				string filePath = string.Empty;
				if (dto.Photo != null && dto.Photo.Length > 0)
					filePath = await _supabaseFileService.UploadFileSaveVersionAsync(dto.Photo, "user_memory", dto.Id.ToString());

				var memory = new Memory
				{
					Id = dto.Id,
					FilePath = filePath,
				};

				var result = await _repo.UpdateAsync(memory);
				if (result <= 0) return -1;
				return result;
			}

			catch (Exception ex)
			{
				return -1;
			}
		}
	}
}
