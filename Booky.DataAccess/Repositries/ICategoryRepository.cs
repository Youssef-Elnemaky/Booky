using Booky.Models;

namespace Booky.DataAccess.Repositries
{
    public interface ICategoryRepository
    {
        public void Add(Category category);
        public void Update(Category category);
        public void Delete(int id);
        public Category? GetById(int id);
        public List<Category> GetAll();
        public void Save();
    }
}
