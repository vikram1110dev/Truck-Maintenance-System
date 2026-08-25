using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;

namespace Truck_Maintanance_system.Controllers
{
    public class ProfitReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ProfitReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProfitReports
        public async Task<IActionResult> Index(int? month, int? year)
        {
            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            ViewBag.CurrentMonth = targetMonth;
            ViewBag.CurrentYear = targetYear;

            // 1. Get Trips for the month
            var trips = await _context.TripRecords
                .Include(t => t.Truck)
                .Where(t => t.EndDate.Month == targetMonth && t.EndDate.Year == targetYear)
                .ToListAsync();

            decimal totalRevenue = trips.Sum(t => t.FreightRevenue);
            decimal tripExpenses = trips.Sum(t => t.FuelCost + t.TollCost + t.DriverAllowance + t.OtherExpenses);

            // 2. Get Maintenance Costs for the month
            var maintenanceRecords = await _context.MechanicalMaintenanceRecords
                .Where(m => m.DateLogged.Month == targetMonth && m.DateLogged.Year == targetYear)
                .ToListAsync();

            decimal maintenanceCosts = 0;
            foreach (var record in maintenanceRecords)
            {
                maintenanceCosts += (record.EngineOil.Cost ?? 0) + (record.TransmissionOil.Cost ?? 0) +
                                    (record.Coolant.Cost ?? 0) + (record.CrownAxelOil.Cost ?? 0) +
                                    (record.HydraulicOil.Cost ?? 0) + (record.AdBlueDefOil.Cost ?? 0) +
                                    (record.BrakeFluid.Cost ?? 0) + (record.TyreCondition.Cost ?? 0) +
                                    (record.WheelAlignment.Cost ?? 0) + (record.SpareWheelCondition.Cost ?? 0) +
                                    (record.TyrePressure.Cost ?? 0) + (record.AirFilter.Cost ?? 0) +
                                    (record.OilFilter.Cost ?? 0) + (record.FuelFilter.Cost ?? 0) +
                                    (record.AcCabinFilter.Cost ?? 0) + (record.HydraulicFilter.Cost ?? 0) +
                                    (record.WaterSeparatorDieselFilter.Cost ?? 0) + (record.BrakeShoeDiscFront.Cost ?? 0) +
                                    (record.BrakeShoeDiscRear.Cost ?? 0) + (record.BrakeRotorDiscFront.Cost ?? 0) +
                                    (record.BrakeRotorDiscRear.Cost ?? 0) + (record.AirCompressorAndValve.Cost ?? 0) +
                                    (record.Greasing.Cost ?? 0) + (record.ClutchPlateLife.Cost ?? 0) +
                                    (record.BatteryCondition.Cost ?? 0);
            }

            // 3. Get Document Renewal Costs for the month
            var documentRecords = await _context.TruckDocuments
                .Where(d => d.IssueDate.Month == targetMonth && d.IssueDate.Year == targetYear)
                .ToListAsync();

            decimal documentCosts = documentRecords.Sum(d => d.Cost ?? 0);

            // 4. Calculate Net Profit
            decimal totalExpenses = tripExpenses + maintenanceCosts + documentCosts;
            decimal netProfit = totalRevenue - totalExpenses;

            // Pass data to view
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TripExpenses = tripExpenses;
            ViewBag.MaintenanceCosts = maintenanceCosts;
            ViewBag.DocumentCosts = documentCosts;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.NetProfit = netProfit;
            ViewBag.TripsCount = trips.Count;

            return View(trips); // We can list the trips at the bottom of the dashboard
        }
    }
}
