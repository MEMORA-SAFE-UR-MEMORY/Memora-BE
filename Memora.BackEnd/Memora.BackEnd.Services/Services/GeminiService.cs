using Memora.BackEnd.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Memora.BackEnd.Services.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _configuration;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateMemoryDescriptionAsync(string title, DateTime date, byte[] imageData, string mimeType = "image/jpeg")
        {
            var apiKey = _configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("API key cho GoogleAI chưa được cấu hình.");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var httpClient = new HttpClient();

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new object[]
                {
                    new { text = $"Tiêu đề: {title}" },
                    new { text = $"Ngày: {date:dd/MM/yyyy}" },
                    new { text = "Dựa trên tiêu đề và ảnh, viết mô tả ngắn trong 40 chữ." },
                    new
                    {
                        inline_data = new
                        {
                            mime_type = mimeType,
                            data = Convert.ToBase64String(imageData)
                        }
                    }
                }
            }
        }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            // ❌ Nếu Gemini trả lỗi -> throw -> Controller sẽ trả BadRequest
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API lỗi {response.StatusCode}: {errorJson}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates)
                && candidates.GetArrayLength() > 0
                && candidates[0].TryGetProperty("content", out var contentElem)
                && contentElem.TryGetProperty("parts", out var parts)
                && parts.GetArrayLength() > 0
                && parts[0].TryGetProperty("text", out var textElem))
            {
                var text = textElem.GetString() ?? "";

                // Giới hạn 40 từ
                var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 40)
                    text = string.Join(" ", words.Take(40)) + "...";

                return text;
            }

            throw new Exception("Gemini trả về phản hồi không hợp lệ.");
        }
    }
}
