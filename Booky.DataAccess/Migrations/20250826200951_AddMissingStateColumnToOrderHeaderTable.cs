using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingStateColumnToOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "orderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "orderHeaders");
        }
    }
}
