using System.ComponentModel.DataAnnotations;

namespace Memora.BackEnd.Services.Dtos
{
	public class RefreshTokenRequest
	{
		[Required(ErrorMessage = "Refresh token is required!")]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
