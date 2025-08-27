using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;

namespace TuneStore.Application.Abstractions
{
    public interface IOrderService
    {
        Task<Order> CreateAsync(OrderCreateDto dto);
        Task<Order?> GetAsync(int id);
        Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId);
    }
}
