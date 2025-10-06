using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface IAlbumService
	{
		Task<int> UpdateAsync(AlbumSlotDto dto);
	}
}
