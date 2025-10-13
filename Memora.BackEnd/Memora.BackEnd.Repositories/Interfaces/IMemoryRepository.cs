using Memora.BackEnd.Repositories.Models;

namespace Memora.BackEnd.Repositories.Interfaces
{
	public interface IMemoryRepository
	{
		Task<int> UpdateAsync(Memory memory);
	}
}
