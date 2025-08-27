using Microsoft.EntityFrameworkCore;
using TuneStore.Domain.Entities;

namespace TuneStore.Infrastructure.Data
{
    public class TuneStoreDbContext : DbContext
    {
        public TuneStoreDbContext(DbContextOptions<TuneStoreDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public object Albums { get; internal set; }
        public object Users { get; internal set; }
    }
}
