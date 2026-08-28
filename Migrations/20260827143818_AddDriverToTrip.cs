using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "TripRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripRecords_DriverId",
                table: "TripRecords",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripRecords_AspNetUsers_DriverId",
                table: "TripRecords",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripRecords_AspNetUsers_DriverId",
                table: "TripRecords");

            migrationBuilder.DropIndex(
                name: "IX_TripRecords_DriverId",
                table: "TripRecords");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "TripRecords");
        }
    }
}
