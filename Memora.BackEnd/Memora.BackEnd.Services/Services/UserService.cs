using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Libraries;
using Microsoft.Extensions.Options;

namespace Memora.BackEnd.Services.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly JWTSettings _jwtSettings;

		public UserService(IUserRepository userRepository, IOptions<JWTSettings> jwtSettings)
		{
			_userRepository = userRepository;
			_jwtSettings = jwtSettings.Value
				?? throw new ArgumentNullException(nameof(jwtSettings), "JWT settings is not configured");
		}
		public async Task<(string accessToken, string refreshToken)?> LoginAsync(string userName, string password)
		{
			var user = await _userRepository.GetByUsernameAsync(userName);
			if (user is null) return null;
			if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

			var accessToken = Authentication.CreateAccessToken(user, _jwtSettings);
			var refreshToken = Authentication.CreateRefreshToken(user, _jwtSettings);
			user.RefreshToken = refreshToken;
			var result = await _userRepository.UpdateUserAsync(user);
			if (result < 0) return null;
			return (accessToken, refreshToken);
		}

		public async Task<int> RegisterAsync(string userName, string password)
		{
			var existingUser = await _userRepository.GetByUsernameAsync(userName);
			if (existingUser is not null) return -1;

			var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
			var user = new User()
			{
				Username = userName,
				PasswordHash = passwordHash,
				RoleId = 1,
			};

			return await _userRepository.CreateUserAsync(user);
		}

		public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken)
		{
			var principal = Authentication.ValidateToken(refreshToken, _jwtSettings);
			if (principal == null)
			{
				return null; 
			}

			var tokenType = principal.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
			if (tokenType != "refresh")
			{
				return null;
			}

			var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

			if (user == null)
			{
				return null;
			}

			var newAccessToken = Authentication.CreateAccessToken(user, _jwtSettings);
			var newRefreshToken = Authentication.CreateRefreshToken(user, _jwtSettings);

			user.RefreshToken = newRefreshToken;
			await _userRepository.UpdateUserAsync(user);

			return (newAccessToken, newRefreshToken);
		}
	}
}

