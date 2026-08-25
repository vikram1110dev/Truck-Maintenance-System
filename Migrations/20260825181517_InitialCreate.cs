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
                    ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: false),
                    EngineOil_Status = table.Column<string>(type: "TEXT", nullable: false),
                    EngineOil_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    EngineOil_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransmissionOil_Status = table.Column<string>(type: "TEXT", nullable: false),
                    TransmissionOil_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    TransmissionOil_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Coolant_Status = table.Column<string>(type: "TEXT", nullable: false),
                    Coolant_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    Coolant_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CrownAxelOil_Status = table.Column<string>(type: "TEXT", nullable: false),
                    CrownAxelOil_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    CrownAxelOil_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HydraulicOil_Status = table.Column<string>(type: "TEXT", nullable: false),
                    HydraulicOil_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    HydraulicOil_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AdBlueDefOil_Status = table.Column<string>(type: "TEXT", nullable: false),
                    AdBlueDefOil_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    AdBlueDefOil_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BrakeFluid_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeFluid_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeFluid_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TyreCondition_Status = table.Column<string>(type: "TEXT", nullable: false),
                    TyreCondition_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    TyreCondition_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WheelAlignment_Status = table.Column<string>(type: "TEXT", nullable: false),
                    WheelAlignment_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    WheelAlignment_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SpareWheelCondition_Status = table.Column<string>(type: "TEXT", nullable: false),
                    SpareWheelCondition_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    SpareWheelCondition_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TyrePressure_Status = table.Column<string>(type: "TEXT", nullable: false),
                    TyrePressure_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    TyrePressure_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AirFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    AirFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    AirFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OilFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    OilFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    OilFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FuelFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    FuelFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    FuelFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcCabinFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    AcCabinFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    AcCabinFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HydraulicFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    HydraulicFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    HydraulicFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WaterSeparatorDieselFilter_Status = table.Column<string>(type: "TEXT", nullable: false),
                    WaterSeparatorDieselFilter_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    WaterSeparatorDieselFilter_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BrakeShoeDiscFront_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeShoeDiscFront_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeShoeDiscFront_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BrakeShoeDiscRear_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeShoeDiscRear_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeShoeDiscRear_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BrakeRotorDiscFront_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeRotorDiscFront_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeRotorDiscFront_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BrakeRotorDiscRear_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BrakeRotorDiscRear_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrakeRotorDiscRear_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AirCompressorAndValve_Status = table.Column<string>(type: "TEXT", nullable: false),
                    AirCompressorAndValve_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    AirCompressorAndValve_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Greasing_Status = table.Column<string>(type: "TEXT", nullable: false),
                    Greasing_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    Greasing_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClutchPlateLife_Status = table.Column<string>(type: "TEXT", nullable: false),
                    ClutchPlateLife_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    ClutchPlateLife_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BatteryCondition_Status = table.Column<string>(type: "TEXT", nullable: false),
                    BatteryCondition_ValidForNextKm = table.Column<int>(type: "INTEGER", nullable: true),
                    BatteryCondition_ValidForNextDate = table.Column<DateTime>(type: "TEXT", nullable: true)
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
