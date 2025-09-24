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
		public async Task<string?> LoginAsync(string userName, string password)
		{
			var user = await _userRepository.GetByUsernameAsync(userName);
			if (user is null) return null;
			if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

			return Authentication.CreateAccessToken(user, _jwtSettings);
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
	}
}
