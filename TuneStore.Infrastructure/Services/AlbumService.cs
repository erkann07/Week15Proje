using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;
using TuneStore.Infrastructure.Data;

namespace TuneStore.Infrastructure.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly TuneStoreDbContext _ctx;
        private readonly IUnitOfWork _uow;
        private readonly IMemoryCache _cache;

        public AlbumService(TuneStoreDbContext ctx, IUnitOfWork uow, IMemoryCache cache)
        {
            _ctx = ctx;
            _uow = uow;
            _cache = cache;
        }

        // Cache ve sayfalama ile albümleri getir
        public async Task<(IReadOnlyList<Album> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var key = $"albums:{page}:{pageSize}:{search}";
            if (_cache.TryGetValue(key, out (IReadOnlyList<Album>, int) cached))
                return cached;

            var query = _ctx.Albums.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a => a.Title.Contains(search) || a.Artist.Contains(search));

            var total = await query.CountAsync();
            var items = await query.OrderBy(a => a.Title)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var result = (items as IReadOnlyList<Album>, total);
            _cache.Set(key, result, TimeSpan.FromMinutes(2));

            return result;
        }

        // Tek bir albüm getir
        public Task<Album?> GetAsync(int id) => _uow.Albums.GetByIdAsync(id);

        // Albüm oluştur
        public async Task<Album> CreateAsync(AlbumCreateUpdateDto dto)
        {
            var a = new Album
            {
                Title = dto.Title,
                Artist = dto.Artist,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };
            await _uow.Albums.AddAsync(a);
            await _uow.SaveChangesAsync();
            return a;
        }

        // Albüm güncelle
        public async Task<Album> UpdateAsync(int id, AlbumCreateUpdateDto dto)
        {
            var a = await _uow.Albums.GetByIdAsync(id) ?? throw new KeyNotFoundException();
            a.Title = dto.Title;
            a.Artist = dto.Artist;
            a.Price = dto.Price;
            a.StockQuantity = dto.StockQuantity;

            _uow.Albums.Update(a);
            await _uow.SaveChangesAsync();
            return a;
        }

        // Stok miktarını değiştir
        public async Task<Album> PatchStockAsync(int id, int delta)
        {
            var a = await _uow.Albums.GetByIdAsync(id) ?? throw new KeyNotFoundException();
            var newQty = a.StockQuantity + delta;
            if (newQty < 0)
                throw new InvalidOperationException("Stock cannot be negative");

            a.StockQuantity = newQty;
            _uow.Albums.Update(a);
            await _uow.SaveChangesAsync();
            return a;
        }

        // Albüm sil
        public async Task DeleteAsync(int id)
        {
            var a = await _uow.Albums.GetByIdAsync(id) ?? throw new KeyNotFoundException();
            _uow.Albums.Remove(a);
            await _uow.SaveChangesAsync();
        }
    }
}
