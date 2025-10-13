using Memora.BackEnd.Services.Dtos;

namespace Memora.BackEnd.Services.Interfaces
{
	public interface IMemoryService
	{
		Task<int> UpdateAsync(ImageRequest dto);
	}
}
