using Memora.BackEnd.Repositories.Models;
using Memora.BackEnd.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memora.BackEnd.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<int> UpdateOrderAsync(UpdateOrderRequest request);
    }
}
