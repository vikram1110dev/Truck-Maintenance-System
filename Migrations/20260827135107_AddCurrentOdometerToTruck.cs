using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentOdometerToTruck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentOdometer",
                table: "Trucks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentOdometer",
                table: "Trucks");
        }
    }
}
