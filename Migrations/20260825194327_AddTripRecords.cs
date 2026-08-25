using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddTripRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: false),
                    RouteStart = table.Column<string>(type: "TEXT", nullable: false),
                    RouteEnd = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FreightRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    FuelCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    TollCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    DriverAllowance = table.Column<decimal>(type: "TEXT", nullable: false),
                    OtherExpenses = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripRecords_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripRecords_TruckId",
                table: "TripRecords",
                column: "TruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripRecords");
        }
    }
}
