using Memora.BackEnd.Repositories.Entities;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Memora.BackEnd.Services.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly string _jwtSecret;

		public UserService(IUserRepository userRepository, string jwtSecret)
		{
			_userRepository = userRepository;
			_jwtSecret = jwtSecret;
		}
		public async Task<string?> LoginAsync(string userName, string password)
		{
			var user = await _userRepository.GetByUsernameAsync(userName);
			if (user is null) return null;
			if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

			return GenerateJwt(user.Id, user.UserName, user.RoleId);	

		}

		public async Task RegisterAsync(string userName, string password)
		{
			var hash = BCrypt.Net.BCrypt.HashPassword(password);
			var user = new User(Guid.NewGuid(), userName, hash, 1);
			await _userRepository.CreateUserAsync(user);
		}

		private string GenerateJwt(Guid? id, string userName, int roleId)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
			new Claim("username", userName),
			new Claim("roleid", roleId.ToString()),
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: "memora-api",
				audience: "memora-client",
				claims: claims,
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: creds);

			var result = new JwtSecurityTokenHandler().WriteToken(token);
			return result;
		}
	}
}
