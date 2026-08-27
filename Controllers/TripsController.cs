using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class TripsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TripsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            var trips = await _context.TripRecords
                .Include(t => t.Truck)
                .Include(t => t.Driver)
                .OrderByDescending(t => t.EndDate)
                .ToListAsync();
            return View(trips);
        }

        // GET: Trips/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            ViewBag.Drivers = await _userManager.GetUsersInRoleAsync("Driver");
            return View();
        }

        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TripRecord trip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            ViewBag.Drivers = await _userManager.GetUsersInRoleAsync("Driver");
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trip = await _context.TripRecords
                .Include(t => t.Truck)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (trip == null) return NotFound();

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.TripRecords.FindAsync(id);
            if (trip != null)
            {
                _context.TripRecords.Remove(trip);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Trips/FuelAnalytics
        public async Task<IActionResult> FuelAnalytics()
        {
            var trips = await _context.TripRecords.Include(t => t.Truck).ToListAsync();
            
            var viewModel = new Truck_Maintanance_system.Models.ViewModels.FuelAnalyticsViewModel();
            
            var groupedTrips = trips.Where(t => t.Truck != null).GroupBy(t => t.TruckId);
            
            foreach (var group in groupedTrips)
            {
                var truck = group.First().Truck!;
                var stat = new Truck_Maintanance_system.Models.ViewModels.TruckFuelStat
                {
                    TruckId = truck.Id,
                    TruckIdentifier = $"{truck.Make} {truck.Model} ({truck.LicensePlate})",
                    TotalTrips = group.Count(),
                    TotalDistance = group.Sum(t => t.DistanceKm),
                    TotalFuelVolume = group.Sum(t => t.FuelVolumeLiters)
                };
                viewModel.TruckStats.Add(stat);
                
                viewModel.FleetTotalDistance += stat.TotalDistance;
                viewModel.FleetTotalFuelVolume += stat.TotalFuelVolume;
            }
            
            // Sort by efficiency descending
            viewModel.TruckStats = viewModel.TruckStats.OrderByDescending(t => t.AverageEfficiency).ToList();

            return View(viewModel);
        }
    }
}
