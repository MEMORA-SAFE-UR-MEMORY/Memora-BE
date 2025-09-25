namespace Memora.BackEnd.Services.Dtos
{
    public class UserOrderDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Fullname { get; set; }
    }
}
