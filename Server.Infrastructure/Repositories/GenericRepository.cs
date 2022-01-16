using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Application.Contracts;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ServerDbContext _context;
        private readonly DbSet<T> _db;
        public GenericRepository(ServerDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, bool disableTracking = true)
        {
            IQueryable<T> query = _db;
            if (disableTracking) query = query.AsNoTracking();
            return await query.Where(predicate).ToListAsync();
        }

        // public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        // {
        //     IQueryable<T> query = _db;
        //     if (disableTracking) query = query.AsNoTracking();

        //     if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

        //     if (predicate != null) query = query.Where(predicate);

        //     if (orderBy != null)
        //         return await orderBy(query).ToListAsync();
        //     return await query.ToListAsync();
        // }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _db;
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate, List<Expression<Func<T, object>>> includes = null)
        {
            IQueryable<T> query = _db;
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _db.AddAsync(entity);
            // await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            // _dbContext.Entry(entity).State = EntityState.Modified;
            // await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _db.Remove(entity);
            // await _dbContext.SaveChangesAsync();
        }
    }
}