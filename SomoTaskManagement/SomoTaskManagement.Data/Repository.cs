using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SomoTaskManagement.Data.Abtract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly SomoTaskManagemnetContext _context;

        public Repository(SomoTaskManagemnetContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression )
        //{
        //    if (expression == null)
        //    {
        //        return await _context.Set<T>().ToListAsync();
        //    }
        //    return await _context.Set<T>().Where(expression).ToListAsync();
        //}

        public async Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (expression == null)
            {
                return await query.ToListAsync();
            }

            return await query.Where(expression).ToListAsync();
        }


        public async Task<T> GetById(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task Add(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task<T> GetSingleByCondition(Expression<Func<T,bool>> expression)
        {
            return await _context.Set<T>().FirstOrDefaultAsync();
        }

        public void Delete(T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            var entities = _context.Set<T>().Where(expression).ToList();
            if (entities.Count > 0)
            {
                _context.Set<T>().RemoveRange(entities);
            }
        }

        public void Update(T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }


        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }
    }
}
