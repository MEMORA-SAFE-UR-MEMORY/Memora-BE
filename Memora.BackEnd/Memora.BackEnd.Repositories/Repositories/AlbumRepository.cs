using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class AlbumRepository : IAlbumRepository
	{
		private readonly PostgresContext _context;
		public AlbumRepository(PostgresContext context)
		{
			_context = context;
		}
		public async Task<Album?> GetByIdAsync(long id)
		{
			return await _context.Albums.FindAsync(id);
		}
	}
}
