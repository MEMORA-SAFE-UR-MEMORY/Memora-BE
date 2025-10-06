using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IAlbumRepository
	{
		Task<int> UpdateAsync(AlbumPageSlot albumPageSlot);
	}
}
