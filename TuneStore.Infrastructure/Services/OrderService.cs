using Microsoft.EntityFrameworkCore;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;
using TuneStore.Infrastructure.Data;
using System;

namespace TuneStore.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly TuneStoreDbContext _ctx;
        private readonly IUnitOfWork _uow;
        public OrderService(TuneStoreDbContext ctx, IUnitOfWork uow) { _ctx = ctx; _uow = uow; }

        public TuneStoreDbContext Ctx => _ctx;

        public async Task<Order> CreateAsync(OrderCreateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(dto.CustomerId) ?? throw new KeyNotFoundException("Customer not found");

            var order = new Order { CustomerId = user.Id, OrderDate = DateTime.UtcNow };
            await _uow.Orders.AddAsync(order);

            decimal total = 0m;
            foreach (var item in dto.Items)
            {
                var album = await _uow.Albums.GetByIdAsync(item.AlbumId) ?? throw new KeyNotFoundException("Album not found");
                if (album.StockQuantity < item.Quantity) throw new InvalidOperationException($"Insufficient stock for album {album.Title}");
                album.StockQuantity -= item.Quantity;
                _uow.Albums.Update(album);

                await _uow.OrderAlbums.AddAsync(new OrderAlbum
                {
                    Order = order,
                    Album = album,
                    Quantity = item.Quantity
                });
                total += album.Price * item.Quantity;
            }
            order.TotalAmount = total;
            await _uow.SaveChangesAsync();
            return order;
        }

        public Task<Order?> GetAsync(int id) => Ctx.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderAlbums).ThenInclude(oa => oa.Album)
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId) => await Ctx.Orders
            .Include(o => o.OrderAlbums)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();
    }
}
