using System.ComponentModel.DataAnnotations;

namespace Memora.BackEnd.Services.Dtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "User name is required!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Please enter a valid email!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string PasswordHash { get; set; }

    }
}
