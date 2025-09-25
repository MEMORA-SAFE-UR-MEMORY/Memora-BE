using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


namespace Memora.BackEnd.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService) => _userService = userService;

		[HttpPost("login")]	
		public async Task<IActionResult> Login([FromBody] AccessRequest request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var tokens = await _userService.LoginAsync(request.UserName, request.Password);
			if (tokens == null) return BadRequest("Invalid login");
			return Ok(new { accessToken = tokens.Value.accessToken, refreshToken = tokens.Value.refreshToken });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] AccessRequest request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _userService.RegisterAsync(request.UserName, request.Password);
			if (result == -1) return BadRequest("Username already exists");
			return Ok("User registered");
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			
			if (string.IsNullOrWhiteSpace(request.RefreshToken))
			{
				return BadRequest("Refresh token is required.");
			}

			var newTokens = await _userService.RefreshTokenAsync(request.RefreshToken);

			if (newTokens == null)
			{
				return Unauthorized("Invalid or expired refresh token.");
			}

			return Ok(new { accessToken = newTokens.Value.accessToken, refreshToken = newTokens.Value.refreshToken });
		}
	}
}
