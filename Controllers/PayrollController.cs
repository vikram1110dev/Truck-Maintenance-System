using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models.ViewModels;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PayrollController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PayrollController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? month, int? year)
        {
            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            ViewBag.CurrentMonth = targetMonth;
            ViewBag.CurrentYear = targetYear;

            var drivers = await _userManager.GetUsersInRoleAsync("Driver");

            // FIXED: Server-side filtering with date range instead of loading all trips
            var monthStart = new DateTime(targetYear, targetMonth, 1);
            var monthEnd = monthStart.AddMonths(1);

            var driverIds = drivers.Select(d => d.Id).ToList();
            var trips = await _context.TripRecords
                .Where(t => t.DriverId != null && driverIds.Contains(t.DriverId)
                            && t.EndDate >= monthStart && t.EndDate < monthEnd)
                .ToListAsync();
            
            var viewModel = new DriverPayrollViewModel();
            
            foreach (var driver in drivers)
            {
                var driverTrips = trips.Where(t => t.DriverId == driver.Id).ToList();
                
                var stat = new DriverPayrollStat
                {
                    DriverId = driver.Id,
                    DriverName = driver.UserName?.Split('@')[0] ?? "Unknown",
                    TotalTrips = driverTrips.Count,
                    TotalDistance = driverTrips.Sum(t => t.DistanceKm),
                    TotalFuelVolume = driverTrips.Sum(t => t.FuelVolumeLiters),
                    TotalSalary = driverTrips.Sum(t => t.DriverAllowance)
                };
                
                viewModel.DriverStats.Add(stat);
            }
            
            viewModel.DriverStats = viewModel.DriverStats.OrderByDescending(d => d.TotalSalary).ToList();

            return View(viewModel);
        }
    }
}
