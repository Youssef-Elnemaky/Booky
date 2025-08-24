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
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            //ISBN is 13 digits right now but just adding extra just in case if they add more digits 
            //like they did when it was 10 and upgraded it to 13
            builder.Property(p => p.ISBN).HasColumnType("VARCHAR(20)");
            builder.Property(p => p.Title).HasColumnType("VARCHAR(200)");
            builder.Property(p => p.Author).HasColumnType("VARCHAR(100)");
            builder.Property(p => p.Description).HasColumnType("NVARCHAR(MAX)");
            builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
            builder.Property(p => p.ListPrice).HasColumnType("DECIMAL(18,2)");
            builder.Property(p => p.Price50).HasColumnType("DECIMAL(18,2)");
            builder.Property(p => p.Price100).HasColumnType("DECIMAL(18,2)");

            // product-category relationship
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            //seeding
            builder.HasData(
                new Product
                {
                    Id = 1,
                    Author = "Jon Skeet",
                    Title = "C# in Depth",
                    ISBN = "9781617291340",
                    Description = "C# in Depth, Third Edition updates the best-selling second edition to cover the new features of C# 5, including the challenges of writing maintainable asynchronous code.",
                    Price = 11,
                    ListPrice = 50,
                    Price50 = 500,
                    Price100 = 1100,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Author = "Andrew Troelsen",
                    Title = "Pro C# 10 and the .NET 6 Platform",
                    ISBN = "9781484278681",
                    Description = "A comprehensive guide to the C# language and the .NET 6 platform, covering everything from core language features to advanced topics.",
                    Price = 15,
                    ListPrice = 60,
                    Price50 = 550,
                    Price100 = 1050,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Author = "Adam Freeman",
                    Title = "ASP.NET Core in Action",
                    ISBN = "9781617298301",
                    Description = "Covers building modern, scalable web applications with ASP.NET Core, including MVC, Razor Pages, Web API, and deployment practices.",
                    Price = 18,
                    ListPrice = 65,
                    Price50 = 600,
                    Price100 = 1150,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Author = "Martin Fowler",
                    Title = "Refactoring: Improving the Design of Existing Code",
                    ISBN = "9780134757599",
                    Description = "Classic book on refactoring techniques that help developers improve the structure and maintainability of existing codebases.",
                    Price = 20,
                    ListPrice = 70,
                    Price50 = 650,
                    Price100 = 1200,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 5,
                    Author = "Robert C. Martin",
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ISBN = "9780132350884",
                    Description = "A must-read for any developer who wants to write cleaner, more maintainable code, with principles, practices, and case studies.",
                    Price = 22,
                    ListPrice = 75,
                    Price50 = 700,
                    Price100 = 1250,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Author = "Eric Evans",
                    Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                    ISBN = "9780321125217",
                    Description = "Seminal work introducing Domain-Driven Design (DDD) principles for handling large-scale and complex software projects.",
                    Price = 25,
                    ListPrice = 80,
                    Price50 = 750,
                    Price100 = 1300,
                    CategoryId = 1
                }
            );

        }
    }
}
