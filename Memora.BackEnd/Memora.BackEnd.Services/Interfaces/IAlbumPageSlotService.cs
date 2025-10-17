using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface IAlbumPageSlotService
	{
		Task<int> UpdateAsync(ImageRequest dto);
	}
}
