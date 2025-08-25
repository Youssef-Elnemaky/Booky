using Booky.DataAccess.Data;
using Booky.DataAccess.Repositries.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repositries
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext context;
        private readonly DbSet<T> dbSet;
        public Repository(AppDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public T Get(Expression<Func<T,bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach(var includeProp in includeProperties
                    .Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.Where(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }
    }
}
