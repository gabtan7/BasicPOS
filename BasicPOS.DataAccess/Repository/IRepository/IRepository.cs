using BasicPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string? includeProperties = null, bool tracked = true);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task Add(T entity);
        void Remove(T entity);
        void UntrackEntity(T entity);
        //void RemoveRange(IEnumerable<T> entity);
    }
}
