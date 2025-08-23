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

        public T Get(Expression<Func<T,bool>> filter)
        {
            return dbSet.Where(filter).FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }
    }
}
