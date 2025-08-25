using Booky.DataAccess.Data;
using Booky.DataAccess.Repositries.IRepository;
using Booky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repositries
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context) : base(context) 
        {
            this.context = context;
        }

        public void Update(Product product)
        {
            var productDb = context.Products.FirstOrDefault(p => p.Id == product.Id);

            if (productDb == null) return;

            productDb.Title = product.Title;
            productDb.Author = product.Author;
            productDb.ISBN = product.ISBN;
            productDb.Description = product.Description;
            productDb.CategoryId = product.CategoryId;
            productDb.ListPrice = product.ListPrice;
            productDb.Price = product.Price;
            productDb.Price50 = product.Price50;
            productDb.Price100 = product.Price100;

            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                productDb.ImageUrl = product.ImageUrl;
            }
            
        }
    }
}
