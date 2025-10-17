using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Memora.BackEnd.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AlbumPageSlotController : ControllerBase
	{
		private readonly IAlbumPageSlotService _albumService;
		public AlbumPageSlotController(IAlbumPageSlotService albumService)
		{
			_albumService = albumService;
		}

		[HttpPut("upload-1-photo")]
		public async Task<IActionResult> UploadPhoto([FromQuery] ImageRequest dto)
		{
			if (ModelState.IsValid)
			{
				var result = await _albumService.UpdateAsync(dto);
				if (result > 0)
				{
					return Ok("Saved Successfully!");
				}
				return BadRequest("Save failed!");
			}
			return BadRequest(ModelState);
		}
	}
}
