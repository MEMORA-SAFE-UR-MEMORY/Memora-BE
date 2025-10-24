using Microsoft.AspNetCore.Http;

namespace Memora.BackEnd.Services.Dtos
{
    public class MemoryRequest
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public IFormFile Image { get; set; }
    }
}
