using BasicPOS.DataAccess.Data;
using BasicPOS.Models;
using BasicPOS.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.DataAccess.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task Add(T entity)
        {
            if (entity.GetType().GetProperty("CreatedDate") != null)
                entity.GetType().GetProperty("CreatedDate").SetValue(entity, DateTime.Now);

            await dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity) //generic soft delete
        {
            entity.GetType().GetProperty("IsActive").SetValue(entity, false);
            dbSet.Update(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            foreach(var t in entity)
            {
                t.GetType().GetProperty("IsActive").SetValue(t, false);
                dbSet.Update(t);
            }
        }

        public void UntrackEntity(T entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }

        //public void RemoveRange(IEnumerable<T> entity)
        //{
        //    dbSet.RemoveRange(entity);
        //}
    }
}
