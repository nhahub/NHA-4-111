using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace FitCore.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        //Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string[]? includes = null);

        Task AddAsync(T entity);
        //Task AddRangeAsync(IEnumerable<T> entities);
        
        void Update(T entity);
        //void UpdateRange(IEnumerable<T> entities);
        
        void Delete(T entity);
        //void DeleteRange(IEnumerable<T> entities);
        
        IQueryable<T> GetAllAsIQueryable();
        IQueryable<T> GetByIdAsIQueryable(int id);
    }
}