using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcCabinFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdBlueDefOil_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirCompressorAndValve_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatteryCondition_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrakeFluid_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrakeRotorDiscFront_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrakeRotorDiscRear_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrakeShoeDiscFront_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrakeShoeDiscRear_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClutchPlateLife_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Coolant_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrownAxelOil_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineOil_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Greasing_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HydraulicFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HydraulicOil_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OilFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpareWheelCondition_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransmissionOil_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TyreCondition_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TyrePressure_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaterSeparatorDieselFilter_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WheelAlignment_Notes",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcCabinFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AdBlueDefOil_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AirCompressorAndValve_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AirFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BatteryCondition_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeFluid_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeRotorDiscFront_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeRotorDiscRear_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeShoeDiscFront_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeShoeDiscRear_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "ClutchPlateLife_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Coolant_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "CrownAxelOil_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "EngineOil_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "FuelFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Greasing_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "HydraulicFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "HydraulicOil_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "OilFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "SpareWheelCondition_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TransmissionOil_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TyreCondition_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TyrePressure_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WaterSeparatorDieselFilter_Notes",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WheelAlignment_Notes",
                table: "MechanicalMaintenanceRecords");
        }
    }
}
