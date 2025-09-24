using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


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
			var token = await _userService.LoginAsync(username, password);
			if (token == null) return BadRequest("Invalid login");
			return Ok(new { token });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string password)
		{
			var result = await _userService.RegisterAsync(username, password);
			if (result == -1) return BadRequest("Username already exists");
			return Ok("User registered");
		}

	}
}
