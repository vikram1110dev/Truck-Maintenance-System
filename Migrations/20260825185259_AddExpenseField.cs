using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AcCabinFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdBlueDefOil_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AirCompressorAndValve_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AirFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BatteryCondition_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BrakeFluid_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BrakeRotorDiscFront_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BrakeRotorDiscRear_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BrakeShoeDiscFront_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BrakeShoeDiscRear_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClutchPlateLife_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Coolant_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CrownAxelOil_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineOil_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FuelFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Greasing_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HydraulicFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HydraulicOil_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OilFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpareWheelCondition_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransmissionOil_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TyreCondition_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TyrePressure_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterSeparatorDieselFilter_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WheelAlignment_Cost",
                table: "MechanicalMaintenanceRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcCabinFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AdBlueDefOil_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AirCompressorAndValve_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "AirFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BatteryCondition_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeFluid_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeRotorDiscFront_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeRotorDiscRear_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeShoeDiscFront_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "BrakeShoeDiscRear_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "ClutchPlateLife_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Coolant_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "CrownAxelOil_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "EngineOil_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "FuelFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Greasing_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "HydraulicFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "HydraulicOil_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "OilFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "SpareWheelCondition_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TransmissionOil_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TyreCondition_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TyrePressure_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WaterSeparatorDieselFilter_Cost",
                table: "MechanicalMaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WheelAlignment_Cost",
                table: "MechanicalMaintenanceRecords");
        }
    }
}
