using System.ComponentModel.DataAnnotations;

namespace Memora.BackEnd.Services.Dtos
{
    public class OrderAlbumDto
    {
        public long Id { get; set; }

        public AlbumDto? AlbumDto { get; set; }

        public long Quantity { get; set; }

        public decimal Price { get; set; }
    }

    public class AlbumDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public AlbumTemplateDto? Template { get; set; }
    }

    public class AlbumTemplateDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class OrderDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserOrderDto? UserInfo { get; set; }
        public List<OrderAlbumDto> OrderAlbums { get; set; } = new();
    }

    // request khi tạo mới
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Trạng thái order là bắt buộc")]
        [StringLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string Status { get; set; } = null!;

        [Range(1, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn 0")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc")]
        public Guid UserId { get; set; }

        [MinLength(1, ErrorMessage = "Phải có ít nhất một album trong order")]
        public List<CreateOrderAlbumRequest> OrderAlbums { get; set; } = new();
    }

    public class CreateOrderAlbumRequest
    {
        [Required(ErrorMessage = "AlbumId là bắt buộc")]
        public long AlbumId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1")]
        public long Quantity { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }
    }

    // request khi update
    public class UpdateOrderRequest
    {
        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "Trạng thái order là bắt buộc")]
        [StringLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string Status { get; set; } = null!;

    }
}
