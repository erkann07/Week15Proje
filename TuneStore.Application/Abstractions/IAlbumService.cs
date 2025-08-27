using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;

namespace TuneStore.Application.Abstractions
{
    public interface IAlbumService
    {
        Task<(IReadOnlyList<Album> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search);
        Task<Album?> GetAsync(int id);
        Task<Album> CreateAsync(AlbumCreateUpdateDto dto);
        Task<Album> UpdateAsync(int id, AlbumCreateUpdateDto dto);
        Task<Album> PatchStockAsync(int id, int delta);
        Task DeleteAsync(int id);
    }
}
