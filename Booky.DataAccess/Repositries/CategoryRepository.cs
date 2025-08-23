using Booky.Models;
using Booky.DataAccess.Data;

namespace Booky.DataAccess.Repositries
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(Category category)
        {
            context.Add(category);
        }

        public void Delete(int id)
        {
            Category? category = context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                context.Categories.Remove(category);
            }
        }

        public List<Category> GetAll()
        {
            return context.Categories.ToList();
        }

        public Category? GetById(int id)
        {
            return context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Category category)
        {
            context.Update(category);
        }
    }
}
