using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IAlbumPageSlotRepository
	{
		Task<int> UpdateAsync(AlbumPageSlot albumPageSlot);
	}
}
