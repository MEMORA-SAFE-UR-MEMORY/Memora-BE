using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace Memora.BackEnd.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService) => _userService = userService;

		[HttpGet("login")]	
		public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
		{
			var tokens = await _userService.LoginAsync(username, password);
			if (tokens == null) return BadRequest("Invalid login");
			return Ok(new { accessToken = tokens.Value.accessToken, refreshToken = tokens.Value.refreshToken });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string password)
		{
			var result = await _userService.RegisterAsync(username, password);
			if (result == -1) return BadRequest("Username already exists");
			return Ok("User registered");
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromQuery][Required] string request)
		{
			if (string.IsNullOrWhiteSpace(request))
			{
				return BadRequest("Refresh token is required.");
			}

			var newTokens = await _userService.RefreshTokenAsync(request);

			if (newTokens == null)
			{
				return Unauthorized("Invalid or expired refresh token.");
			}

			return Ok(new { accessToken = newTokens.Value.accessToken, refreshToken = newTokens.Value.refreshToken });
		}
	}
}
