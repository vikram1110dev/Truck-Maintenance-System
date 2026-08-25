using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vin = table.Column<string>(type: "TEXT", nullable: false),
                    LicensePlate = table.Column<string>(type: "TEXT", nullable: false),
                    Make = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MechanicalMaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateLogged = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OdometerKm = table.Column<int>(type: "INTEGER", nullable: false),
                    EngineOil = table.Column<string>(type: "TEXT", nullable: false),
                    TransmissionOil = table.Column<string>(type: "TEXT", nullable: false),
                    Coolant = table.Column<string>(type: "TEXT", nullable: false),
                    CrownAxelOil = table.Column<string>(type: "TEXT", nullable: false),
                    HydraulicOil = table.Column<string>(type: "TEXT", nullable: false),
                    AdBlueDefOil = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeFluid = table.Column<string>(type: "TEXT", nullable: false),
                    TyreConditionPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    WheelAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    SpareWheelCondition = table.Column<string>(type: "TEXT", nullable: false),
                    TyrePressure = table.Column<string>(type: "TEXT", nullable: false),
                    AirFilter = table.Column<string>(type: "TEXT", nullable: false),
                    OilFilter = table.Column<string>(type: "TEXT", nullable: false),
                    FuelFilter = table.Column<string>(type: "TEXT", nullable: false),
                    AcCabinFilter = table.Column<string>(type: "TEXT", nullable: false),
                    HydraulicFilter = table.Column<string>(type: "TEXT", nullable: false),
                    WaterSeparatorDieselFilter = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeShoeDiscFront = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeShoeDiscRear = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeRotorDiscFront = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeRotorDiscRear = table.Column<string>(type: "TEXT", nullable: false),
                    AirCompressorAndValve = table.Column<string>(type: "TEXT", nullable: false),
                    Greasing = table.Column<string>(type: "TEXT", nullable: false),
                    ClutchPlateLifePercent = table.Column<int>(type: "INTEGER", nullable: false),
                    ClutchPlateAgeMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    BatteryConditionPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    BatteryAgeMonths = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MechanicalMaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MechanicalMaintenanceRecords_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MechanicalMaintenanceRecords_TruckId",
                table: "MechanicalMaintenanceRecords",
                column: "TruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MechanicalMaintenanceRecords");

            migrationBuilder.DropTable(
                name: "Trucks");
        }
    }
}
