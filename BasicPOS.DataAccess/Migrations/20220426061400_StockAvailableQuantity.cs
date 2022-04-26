using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicPOS.DataAccess.Migrations
{
    public partial class StockAvailableQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "Stocks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "Stocks");
        }
    }
}
