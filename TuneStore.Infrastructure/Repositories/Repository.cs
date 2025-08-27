using Microsoft.EntityFrameworkCore;
using TuneStore.Application.Abstractions;
using System.Linq.Expressions;
using TuneStore.Infrastructure.Data;


namespace TuneStore.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TuneStoreDbContext _ctx;
        private TuneStoreDbContext ctx;

        public Repository(TuneStoreDbContext ctx) => _ctx = ctx;

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _ctx.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _ctx.Set<T>().ToListAsync();
        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _ctx.Set<T>().Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);
        public void Update(T entity) => _ctx.Set<T>().Update(entity);
        public void Remove(T entity) => _ctx.Set<T>().Remove(entity);
    }
}
