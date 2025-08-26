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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext _context;

        public ShoppingCartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ShoppingCart cart)
        {
            _context.Update(cart);
        }
    }
}
