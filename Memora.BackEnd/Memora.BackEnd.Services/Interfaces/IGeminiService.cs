namespace Memora.BackEnd.Services.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateMemoryDescriptionAsync(string title, DateTime date, byte[] imageData, string mimeType = "image/jpeg");

    }
}
