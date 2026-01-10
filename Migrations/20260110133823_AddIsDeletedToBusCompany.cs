using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRideBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToBusCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BusCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BusCompanies");
        }
    }
}
