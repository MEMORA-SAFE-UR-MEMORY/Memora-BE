using Memora.BackEnd.Repositories.Base;
using Memora.BackEnd.Repositories.Interfaces;
using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using Memora.BackEnd.Services.Interfaces;
using Memora.BackEnd.Services.Libraries;
using Microsoft.Extensions.Options;

namespace Memora.BackEnd.Services.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly JWTSettings _jwtSettings;
        private readonly EmailService _email;

        public UserService(IUserRepository userRepository, IOptions<JWTSettings> jwtSettings, EmailService email)
		{
			_userRepository = userRepository;
			_jwtSettings = jwtSettings.Value
				?? throw new ArgumentNullException(nameof(jwtSettings), "JWT settings is not configured");
            _email = email;
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

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
			var dtos = users.Select(u => new UserDto
			{
				Email = u.Email,
				Fullname = u.Fullname,
				Id = u.Id,
				Status = u.Status,
				Username = u.Username
			}).ToList();

			return dtos;
        }

        public async Task<int> BanUser(Guid userId)
        {
			return await _userRepository.BanUser(userId);
        }

        public async Task<int> SignUpAsync(string email, string userName, string password)
        {
            var checkUsername = await _userRepository.GetByUsernameAsync(userName);
            if (checkUsername is not null) return -1;
			var checkEmail = await _userRepository.CheckEmailUsername(email);
			if (checkEmail is not null) return -1;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User()
            {
				Email = email,
                Username = userName,
                PasswordHash = passwordHash,
                RoleId = 1,
            };

            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<int> ForgotPassword(string usernameOrEmail)
        {
            var user = await _userRepository.CheckEmailUsername(usernameOrEmail);
            if (user == null) return -1;

            // tạo OTP 6 số
            var rng = new Random();
            var otp = rng.Next(100000, 999999).ToString();

			user.ResetToken = otp;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            var res = await _userRepository.UpdateUserAsync(user);

            string resetLink = $"http://localhost:5000/reset?token={user.ResetToken}";
            await _email.SendEmailAsync(user.Email, "Mã xác thực đặt lại mật khẩu", $"Mã OTP của bạn là: {otp} (hết hạn sau 15 phút)");

            return res;
        }

        public async Task<int> ResetPassword(string otp, string newPass)
        {
            var user = await _userRepository.CheckResetToken(otp);
			if (user == null) return -1;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPass);
			user.PasswordHash = passwordHash;
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
			
			return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> CheckOtp(string otp)
        {
            var user = await _userRepository.CheckResetToken(otp);
            if (user == null) return false;

			return true;
        }
    }
}

