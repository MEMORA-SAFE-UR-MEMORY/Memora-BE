using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;

namespace Memora.BackEnd.Services.Services
{
	public class AlbumPageSlotService : IAlbumPageSlotService
	{
		private readonly IAlbumPageSlotRepository _repo;
		private readonly ISupabaseFileService _supabaseFileService;
		public AlbumPageSlotService(IAlbumPageSlotRepository repo, ISupabaseFileService supabaseFileService)
		{
			_repo = repo;
			_supabaseFileService = supabaseFileService;
		}

		public async Task<int> UpdateAsync(ImageRequest dto)
		{
			try
			{
				string photoUrl = string.Empty;
				if (dto.Photo != null && dto.Photo.Length > 0)
					photoUrl = await _supabaseFileService.UploadFileSaveVersionAsync(dto.Photo, "user_album", dto.Id.ToString());

				var albumSlot = new AlbumPageSlot
				{
					Id = dto.Id,
					PhotoUrl = photoUrl,
				};

				var result = await _repo.UpdateAsync(albumSlot);
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
