using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Repositories.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Memora.BackEnd.Services.Libraries
{
    public static class Authentication
    {
		public static string CreateAccessToken(User user, JWTSettings jwtSettings)
		{
			var now = DateTime.UtcNow;

			var claims = new List<Claim>
			{
				new Claim("id", user.Id.ToString()),
				new Claim("username", user.Username),
				new Claim("role", user.RoleId.ToString()),
				new Claim("token_type", "access")
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey
				?? throw new ArgumentNullException(nameof(jwtSettings.SecretKey))));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings.Issuer,
				audience: jwtSettings.Audience,
				claims: claims,
				notBefore: now,
				expires: now.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string CreateRefreshToken(User user, JWTSettings jwtSettings)
		{
			var now = DateTime.UtcNow;

			var claims = new List<Claim>
			{
				new Claim("id", user.Id.ToString()),
				new Claim("username", user.Username),
				new Claim("role", user.RoleId.ToString()),
				new Claim("token_type", "refresh")
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey
				?? throw new ArgumentNullException(nameof(jwtSettings.SecretKey))));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings.Issuer,
				audience: jwtSettings.Audience,
				claims: claims,
				notBefore: now,
				expires: now.AddDays(jwtSettings.RefreshTokenExpirationDays),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static Guid GetUserId(this ClaimsPrincipal user)
		{
			var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "id");
			return Guid.TryParse(userIdClaim?.Value, out Guid userId) ? userId : Guid.Empty;
		}

		public static string GetUsername(this ClaimsPrincipal user)
		{
			return user.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? string.Empty;
		}

		public static int GetUserRole(this ClaimsPrincipal user)
		{
			var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "role");
			return int.TryParse(roleClaim?.Value, out int role) ? role : 0;
		}
	}
}
