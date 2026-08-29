using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FleetReportsController : Controller
    {
        private readonly AppDbContext _context;

        public FleetReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FleetReports
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var defaultFrom = fromDate ?? DateTime.Today.AddDays(-30);
            var defaultTo = toDate ?? DateTime.Today;

            var trips = await _context.TripRecords
                .Include(t => t.Truck)
                .Where(t => t.StartDate >= defaultFrom && t.StartDate <= defaultTo)
                .ToListAsync();

            var maintenance = await _context.MechanicalMaintenanceRecords
                .Include(m => m.Truck)
                .Where(m => m.DateLogged >= defaultFrom && m.DateLogged <= defaultTo)
                .ToListAsync();

            var fuel = await _context.FuelLogs
                .Include(f => f.Truck)
                .Where(f => f.FuelDate >= defaultFrom && f.FuelDate <= defaultTo)
                .ToListAsync();

            var inspections = await _context.VehicleInspections
                .Include(i => i.Truck)
                .Where(i => i.InspectionDate >= defaultFrom && i.InspectionDate <= defaultTo)
                .ToListAsync();

            var totalRevenue = trips.Sum(t => t.FreightRevenue);
            var totalMaintenanceCost = maintenance.Sum(m => m.TotalCost);
            var totalFuelCost = fuel.Sum(f => f.TotalAmount);
            var netOperatingMargin = totalRevenue - (totalMaintenanceCost + totalFuelCost);

            ViewBag.FromDate = defaultFrom.ToString("yyyy-MM-dd");
            ViewBag.ToDate = defaultTo.ToString("yyyy-MM-dd");
            ViewBag.TotalTrips = trips.Count;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalMaintenanceCost = totalMaintenanceCost;
            ViewBag.TotalFuelCost = totalFuelCost;
            ViewBag.NetMargin = netOperatingMargin;
            ViewBag.TotalInspections = inspections.Count;
            ViewBag.DefectInspections = inspections.Count(i => i.HasDefects);

            return View();
        }

        // GET: FleetReports/ExportTripsCsv
        public async Task<IActionResult> ExportTripsCsv(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.TripRecords.Include(t => t.Truck).AsQueryable();
            if (fromDate.HasValue) query = query.Where(t => t.StartDate >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(t => t.StartDate <= toDate.Value.Date);

            var trips = await query.OrderByDescending(t => t.StartDate).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Trip ID,Date,Truck,Driver,Start Location,End Location,Distance (km),Revenue (INR),Fuel Cost (INR),Status");

            foreach (var t in trips)
            {
                var line = $"{t.Id},{t.StartDate:yyyy-MM-dd},\"{EscapeCsv(t.Truck?.LicensePlate ?? "")}\",\"{EscapeCsv(t.DriverId ?? "")}\",\"{EscapeCsv(t.RouteStart)}\",\"{EscapeCsv(t.RouteEnd)}\",{t.DistanceKm},{t.FreightRevenue},{t.FuelCost},\"{EscapeCsv(t.Status.ToString())}\"";
                csv.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"Trips_Report_{DateTime.Today:yyyyMMdd}.csv");
        }

        // GET: FleetReports/ExportMaintenanceCsv
        public async Task<IActionResult> ExportMaintenanceCsv(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.MechanicalMaintenanceRecords.Include(m => m.Truck).AsQueryable();
            if (fromDate.HasValue) query = query.Where(m => m.DateLogged >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(m => m.DateLogged <= toDate.Value.Date);

            var records = await query.OrderByDescending(m => m.DateLogged).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Record ID,Date,Truck VIN,License Plate,Total Cost (INR),Odometer (km),Engine Oil Status,Brake Fluid Status,Coolant Status");

            foreach (var m in records)
            {
                var line = $"{m.Id},{m.DateLogged:yyyy-MM-dd},\"{EscapeCsv(m.Truck?.Vin ?? "")}\",\"{EscapeCsv(m.Truck?.LicensePlate ?? "")}\",{m.TotalCost},{m.OdometerKm},\"{EscapeCsv(m.EngineOil?.Status ?? "")}\",\"{EscapeCsv(m.BrakeFluid?.Status ?? "")}\",\"{EscapeCsv(m.Coolant?.Status ?? "")}\"";
                csv.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"Maintenance_Report_{DateTime.Today:yyyyMMdd}.csv");
        }

        // GET: FleetReports/ExportFuelLogsCsv
        public async Task<IActionResult> ExportFuelLogsCsv(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.FuelLogs.Include(f => f.Truck).AsQueryable();
            if (fromDate.HasValue) query = query.Where(f => f.FuelDate >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(f => f.FuelDate <= toDate.Value.Date);

            var logs = await query.OrderByDescending(f => f.FuelDate).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Fuel Log ID,Date,Truck,Driver,Liters,Rate / Liter (INR),Total Amount (INR),Odometer (km),Station,Payment Mode,Notes");

            foreach (var f in logs)
            {
                var line = $"{f.Id},{f.FuelDate:yyyy-MM-dd},\"{EscapeCsv(f.Truck?.LicensePlate ?? "")}\",\"{EscapeCsv(f.DriverName)}\",{f.Liters},{f.PricePerLiter},{f.TotalAmount},{f.OdometerReading},\"{EscapeCsv(f.FuelStation ?? "")}\",\"{EscapeCsv(f.PaymentMode)}\",\"{EscapeCsv(f.Notes ?? "")}\"";
                csv.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"Fuel_Logs_Report_{DateTime.Today:yyyyMMdd}.csv");
        }

        // GET: FleetReports/ExportInspectionsCsv
        public async Task<IActionResult> ExportInspectionsCsv(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.VehicleInspections.Include(i => i.Truck).AsQueryable();
            if (fromDate.HasValue) query = query.Where(i => i.InspectionDate >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(i => i.InspectionDate <= toDate.Value.Date);

            var inspections = await query.OrderByDescending(i => i.InspectionDate).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Report ID,Date,Truck,Driver,Type,Odometer (km),Brakes,Lights,Tyres,Fluids,Steering,Wipers,Is Safe,Defects Reported,Remarks");

            foreach (var i in inspections)
            {
                var line = $"{i.Id},{i.InspectionDate:yyyy-MM-dd HH:mm},\"{EscapeCsv(i.Truck?.LicensePlate ?? "")}\",\"{EscapeCsv(i.DriverName)}\",\"{i.Type}\",{i.OdometerReading},{(i.BrakesOk ? "PASS" : "DEFECT")},{(i.LightsAndSignalsOk ? "PASS" : "DEFECT")},{(i.TyresAndWheelsOk ? "PASS" : "DEFECT")},{(i.EngineOilAndFluidsOk ? "PASS" : "DEFECT")},{(i.SteeringAndHornOk ? "PASS" : "DEFECT")},{(i.WipersAndGlassOk ? "PASS" : "DEFECT")},{(i.IsSafeToOperate ? "YES" : "NO")},\"{EscapeCsv(i.DefectsDescription ?? "")}\",\"{EscapeCsv(i.Remarks ?? "")}\"";
                csv.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"Inspection_Audit_Report_{DateTime.Today:yyyyMMdd}.csv");
        }

        private static string EscapeCsv(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Replace("\"", "\"\"");
        }
    }
}
