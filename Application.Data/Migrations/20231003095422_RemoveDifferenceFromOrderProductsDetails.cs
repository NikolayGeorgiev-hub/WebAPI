using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDifferenceFromOrderProductsDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difference",
                table: "OrderProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Difference",
                table: "OrderProducts",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);
        }
    }
}
