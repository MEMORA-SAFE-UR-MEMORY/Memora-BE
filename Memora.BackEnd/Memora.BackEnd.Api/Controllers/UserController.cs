using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Memora.BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccessRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tokens = await _userService.LoginAsync(request.Email, request.Password);
            if (tokens == null) return BadRequest("Invalid username or password");
            if (tokens.Value.status == "Banned") return BadRequest("You are banned, contact us on website for more information");
            return Ok(new { accessToken = tokens.Value.accessToken, refreshToken = tokens.Value.refreshToken });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccessRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.RegisterAsync(request.Email, request.Password);
            if (result == -1) return BadRequest("Username already exists");
            return Ok("User registered");
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "2")]
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "2")]
        [HttpPost("{id:Guid}/ban")]
        public async Task<IActionResult> BanUser([FromRoute] Guid id)
        {
            var result = await _userService.BanUser(id);
            if (result == 0) return BadRequest("Không tìm thấy user");
            return Ok("Đã ban thành công");
        }

        // SignUp
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.SignUpAsync(request.Email, request.Username, request.PasswordHash);
            if (result == -1) return BadRequest("Username or Email already exists");
            return Ok("User registered");
        }

        // Request password reset
        [AllowAnonymous]
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] string usernameOrEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.ForgotPassword(usernameOrEmail);
            if (result == -1) return BadRequest("Username or Email does not exists");
            return Ok("OTP sent");
        }

        // Reset password
        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromQuery] string otp, [FromBody] string newPassword)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.ResetPassword(otp, newPassword);
            if (result == -1) return BadRequest("Token invalid");
            return Ok("Password change complete");
        }

        // Verify OTP
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.CheckOtp(otp);
            if (!result) return BadRequest("OTP invalid");
            return Ok("OTP valid, you can change password");
        }
    }
}