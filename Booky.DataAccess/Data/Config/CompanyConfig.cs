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

            //seeding
            builder.HasData(
                [
                    new Company
                    {
                        Id = 1,
                        Name = "TechNova Solutions",
                        StreetAddress = "123 Innovation Drive",
                        City = "San Francisco",
                        State = "CA",
                        PostalCode = "94105",
                        PhoneNumber = "+1-415-555-1234"
                    },
                    new Company
                    {
                        Id = 2,
                        Name = "GreenLeaf Publishing",
                        StreetAddress = "456 Maple Street",
                        City = "Seattle",
                        State = "WA",
                        PostalCode = "98101",
                        PhoneNumber = "+1-206-555-5678"
                    },
                    new Company
                    {
                        Id = 3,
                        Name = "Global Bookstore Inc.",
                        StreetAddress = "789 Oxford Road",
                        City = "Boston",
                        State = "MA",
                        PostalCode = "02110",
                        PhoneNumber = "+1-617-555-9012"
                    }
                ]);

        }
    }
}
