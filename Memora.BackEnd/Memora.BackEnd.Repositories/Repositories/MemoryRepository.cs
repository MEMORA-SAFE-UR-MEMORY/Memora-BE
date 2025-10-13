using Memora.BackEnd.Repositories.DBContext;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Repositories
{
	public class MemoryRepository : IMemoryRepository
	{
		private readonly PostgresContext _context;

		public MemoryRepository(PostgresContext context)
		{
			_context = context;
		}
		public async Task<int> UpdateAsync(Memory memory)
		{
			var existing = await _context.Memories.FindAsync(memory.Id);
			if (existing == null)
				return -1;

			existing.FilePath = memory.FilePath;
			_context.Update(existing);
			return await _context.SaveChangesAsync();
		}
	}
}
