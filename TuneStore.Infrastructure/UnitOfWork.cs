using TuneStore.Application.Abstractions;
using TuneStore.Domain.Entities;
using TuneStore.Infrastructure.Data;
using TuneStore.Infrastructure.Repositories;


namespace TuneStore.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TuneStoreDbContext _ctx;
        public IRepository<User> Users { get; }
        public IRepository<Album> Albums { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderAlbum> OrderAlbums { get; }
        public IRepository<MaintenanceWindow> MaintenanceWindows { get; }
        public IRepository<RequestLog> RequestLogs { get; }

        public UnitOfWork(TuneStoreDbContext ctx)
        {
            _ctx = ctx;
            Users = new Repository<User>(_ctx);
            Albums = new Repository<Album>(_ctx);
            Orders = new Repository<Order>(_ctx);
            OrderAlbums = new Repository<OrderAlbum>(_ctx);
            MaintenanceWindows = new Repository<MaintenanceWindow>(_ctx);
            RequestLogs = new Repository<RequestLog>(_ctx);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
        public ValueTask DisposeAsync() => _ctx.DisposeAsync();
    }
}
