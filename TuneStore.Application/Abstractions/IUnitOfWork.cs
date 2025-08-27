using TuneStore.Domain.Entities;

namespace TuneStore.Application.Abstractions
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Album> Albums { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderAlbum> OrderAlbums { get; }
        IRepository<MaintenanceWindow> MaintenanceWindows { get; }
        IRepository<RequestLog> RequestLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
