using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemoveJobIdToDiscountModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JodId",
                table: "Discounts",
                newName: "StartJobId");

            migrationBuilder.AddColumn<string>(
                name: "RemoveJobId",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoveJobId",
                table: "Discounts");

            migrationBuilder.RenameColumn(
                name: "StartJobId",
                table: "Discounts",
                newName: "JodId");
        }
    }
}
