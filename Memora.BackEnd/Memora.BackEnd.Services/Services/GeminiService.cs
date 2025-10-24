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
                throw new InvalidOperationException("API key cho GoogleAI chưa được cấu hình trong appsettings.json.");

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var httpClient = new HttpClient();

            var imageBase64 = Convert.ToBase64String(imageData);

            // ✅ Prompt: giới hạn tối đa 40 từ
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
                            new
                            {
                                text = "Dựa trên tiêu đề, ngày, và ảnh được cung cấp, " +
                                       "viết một đoạn văn ngắn, sống động và đầy cảm xúc về ký ức này " +
                                       "trong 40 chữ hoặc ít hơn."
                            },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = mimeType,
                                    data = imageBase64
                                }
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return $"Lỗi khi gọi Gemini API: {response.StatusCode}\n{err}";
            }

            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates)
                    && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var contentElem)
                        && contentElem.TryGetProperty("parts", out var parts)
                        && parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];
                        if (firstPart.TryGetProperty("text", out var textElem))
                        {
                            var text = textElem.GetString() ?? "Không thể trích xuất văn bản từ phản hồi.";

                            // ✅ Giới hạn kết quả tối đa 40 từ nếu Gemini trả quá dài
                            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (words.Length > 40)
                                text = string.Join(" ", words.Take(40)) + "...";

                            return text;
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                return $"Lỗi khi phân tích phản hồi JSON: {ex.Message}";
            }

            return "Không thể tạo miêu tả cho ký ức này.";
        }
    }
}
