using Booky.Models;
using Booky.DataAccess.Data;
using Booky.DataAccess.Repositries.IRepository;

namespace Booky.DataAccess.Repositries
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            this.context = context;
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
