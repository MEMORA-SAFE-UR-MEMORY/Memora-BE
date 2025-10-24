using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Memora.BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        private readonly IMemoryService _memoryService;
        private readonly IGeminiService _geminiService;

        public MemoryController(IMemoryService memoryService, IGeminiService geminiService)
        {
            _memoryService = memoryService;
            _geminiService = geminiService;
        }

        [HttpPut("upload-1-photo")]
        public async Task<IActionResult> UploadPhoto([FromQuery] ImageRequest dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _memoryService.UpdateAsync(dto);
                if (result > 0)
                    return Ok("Saved Successfully!");
                return BadRequest("Save failed!");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("suggest-description")]
        public async Task<IActionResult> SuggestDescription([FromForm] MemoryRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Title) || request.Image == null)
                return BadRequest("Vui lòng cung cấp tiêu đề, ngày và hình ảnh ký ức.");

            var validTypes = new[] { "image/png", "image/jpeg" };
            if (!validTypes.Contains(request.Image.ContentType))
                return BadRequest("Chỉ chấp nhận ảnh PNG hoặc JPEG.");

            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                await request.Image.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
            }

            try
            {
                string description = await _geminiService.GenerateMemoryDescriptionAsync(
                    request.Title,
                    request.Date,
                    imageData,
                    request.Image.ContentType
                );
                return Ok(new { SuggestedDescription = description });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo miêu tả ký ức: {ex.Message}");
            }
        }
    }
}
