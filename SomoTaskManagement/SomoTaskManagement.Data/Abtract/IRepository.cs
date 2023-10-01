using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data.Abtract
{
    public interface IRepository<T> where T : class
    {
        Task Add(T entity);
        Task Add(IEnumerable<T> entities);
        Task Commit();
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> expression);
        Task<T> GetById(object id);
        //Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<T> GetSingleByCondition(Expression<Func<T, bool>> expression);
        void Update(T entity);
    }
}
