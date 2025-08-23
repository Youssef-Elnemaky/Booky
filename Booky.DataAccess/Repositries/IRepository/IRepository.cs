using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repositries.IRepository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T,bool>> filter); 
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
