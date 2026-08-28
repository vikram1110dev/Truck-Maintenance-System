using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
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

            // FIXED: Use date range comparison instead of .Month/.Year which can't be translated to SQL
            var monthStart = new DateTime(targetYear, targetMonth, 1);
            var monthEnd = monthStart.AddMonths(1);

            // 1. Get Trips for the month
            var trips = await _context.TripRecords
                .Include(t => t.Truck)
                .Where(t => t.EndDate >= monthStart && t.EndDate < monthEnd)
                .ToListAsync();

            decimal totalRevenue = trips.Sum(t => t.FreightRevenue);
            decimal tripExpenses = trips.Sum(t => t.FuelCost + t.TollCost + t.DriverAllowance + t.OtherExpenses);

            // 2. Get Maintenance Costs for the month — FIXED: Uses TotalCost computed property
            var maintenanceRecords = await _context.MechanicalMaintenanceRecords
                .Where(m => m.DateLogged >= monthStart && m.DateLogged < monthEnd)
                .ToListAsync();

            decimal maintenanceCosts = maintenanceRecords.Sum(r => r.TotalCost);

            // 3. Get Document Renewal Costs for the month
            var documentRecords = await _context.TruckDocuments
                .Where(d => d.IssueDate >= monthStart && d.IssueDate < monthEnd)
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
