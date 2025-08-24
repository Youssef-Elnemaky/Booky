using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Booky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTableWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "VARCHAR(200)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Author = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    ListPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Price50 = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Price100 = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Jon Skeet", "C# in Depth, Third Edition updates the best-selling second edition to cover the new features of C# 5, including the challenges of writing maintainable asynchronous code.", "9781617291340", 50m, 11m, 1100m, 500m, "C# in Depth" },
                    { 2, "Andrew Troelsen", "A comprehensive guide to the C# language and the .NET 6 platform, covering everything from core language features to advanced topics.", "9781484278681", 60m, 15m, 1050m, 550m, "Pro C# 10 and the .NET 6 Platform" },
                    { 3, "Adam Freeman", "Covers building modern, scalable web applications with ASP.NET Core, including MVC, Razor Pages, Web API, and deployment practices.", "9781617298301", 65m, 18m, 1150m, 600m, "ASP.NET Core in Action" },
                    { 4, "Martin Fowler", "Classic book on refactoring techniques that help developers improve the structure and maintainability of existing codebases.", "9780134757599", 70m, 20m, 1200m, 650m, "Refactoring: Improving the Design of Existing Code" },
                    { 5, "Robert C. Martin", "A must-read for any developer who wants to write cleaner, more maintainable code, with principles, practices, and case studies.", "9780132350884", 75m, 22m, 1250m, 700m, "Clean Code: A Handbook of Agile Software Craftsmanship" },
                    { 6, "Eric Evans", "Seminal work introducing Domain-Driven Design (DDD) principles for handling large-scale and complex software projects.", "9780321125217", 80m, 25m, 1300m, 750m, "Domain-Driven Design: Tackling Complexity in the Heart of Software" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
