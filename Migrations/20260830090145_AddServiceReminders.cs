using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Truck_Maintanance_system.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuelLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FuelDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Liters = table.Column<decimal>(type: "TEXT", nullable: false),
                    PricePerLiter = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OdometerReading = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelStation = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    ReceiptImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentMode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelLogs_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueOdometer = table.Column<int>(type: "INTEGER", nullable: true),
                    LastServicedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastServicedOdometer = table.Column<int>(type: "INTEGER", nullable: true),
                    IntervalMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    IntervalKm = table.Column<int>(type: "INTEGER", nullable: true),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceReminders_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TyreInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: true),
                    AxlePosition = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TreadDepthMm = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    InstallationOdometer = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TyreInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TyreInventories_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TruckId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OdometerReading = table.Column<int>(type: "INTEGER", nullable: false),
                    BrakesOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    LightsAndSignalsOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    TyresAndWheelsOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    EngineOilAndFluidsOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    SteeringAndHornOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    WipersAndGlassOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSafeToOperate = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefectsDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CorrectiveActionTaken = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInspections_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelLogs_FuelDate",
                table: "FuelLogs",
                column: "FuelDate");

            migrationBuilder.CreateIndex(
                name: "IX_FuelLogs_TruckId",
                table: "FuelLogs",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_DueDate",
                table: "ServiceReminders",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_Status",
                table: "ServiceReminders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_TruckId",
                table: "ServiceReminders",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_TyreInventories_SerialNumber",
                table: "TyreInventories",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TyreInventories_TruckId",
                table: "TyreInventories",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_InspectionDate",
                table: "VehicleInspections",
                column: "InspectionDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_TruckId",
                table: "VehicleInspections",
                column: "TruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelLogs");

            migrationBuilder.DropTable(
                name: "ServiceReminders");

            migrationBuilder.DropTable(
                name: "TyreInventories");

            migrationBuilder.DropTable(
                name: "VehicleInspections");
        }
    }
}
