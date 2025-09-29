using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memora.BackEnd.Services.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Status { get; set; }
    }
}
