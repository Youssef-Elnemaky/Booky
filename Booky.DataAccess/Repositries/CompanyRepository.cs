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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext context;

        public CompanyRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        public void Update(Company company)
        {
            // unless you want to do extra work
            context.Update(company);
        }
    }
}
