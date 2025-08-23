using Booky.DataAccess.Data;
using Booky.DataAccess.Repositries.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repositries
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        public ICategoryRepository Category { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
            Category = new CategoryRepository(context);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
