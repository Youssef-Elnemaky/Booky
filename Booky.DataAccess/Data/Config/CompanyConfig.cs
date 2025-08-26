using Booky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Data.Config
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).HasColumnType("VARCHAR(200)").IsRequired();
            builder.Property(c => c.StreetAddress).HasColumnType("VARCHAR(250)").IsRequired(false);
            builder.Property(c => c.City).HasColumnType("VARCHAR(100)").IsRequired(false);
            builder.Property(c => c.State).HasColumnType("VARCHAR(50)").IsRequired(false);
            builder.Property(c => c.PostalCode).HasColumnType("VARCHAR(20)").IsRequired(false);
            builder.Property(c => c.PhoneNumber).HasColumnType("VARCHAR(20)").IsRequired(false);


        }
    }
}
