using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class AlbumPageSlotRepository : IAlbumPageSlotRepository
	{
		private readonly PostgresContext _context;

		public AlbumPageSlotRepository(PostgresContext context)
		{
			_context = context;
		}
		public async Task<int> UpdateAsync(AlbumPageSlot albumPageSlot)
		{
			var existing = await _context.AlbumPageSlots.FindAsync(albumPageSlot.Id);
			if (existing == null)
				return -1;

			existing.PhotoUrl = albumPageSlot.PhotoUrl;
			_context.Update(existing);
			return await _context.SaveChangesAsync();
		}
	}
}
