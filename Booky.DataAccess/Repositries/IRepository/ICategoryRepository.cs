using Booky.Models;

namespace Booky.DataAccess.Repositries.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Save();
        void Update(Category category);
    }
}
