using System.ComponentModel.DataAnnotations;

namespace Memora.BackEnd.Services.Dtos
{
	public class AccessRequest
	{
		[Required(ErrorMessage = "User name is required!")]
		public string UserName { get; set; } = string.Empty;
		[Required(ErrorMessage = "Password is required!")]
		public string Password { get; set; } = string.Empty;
	}
}
