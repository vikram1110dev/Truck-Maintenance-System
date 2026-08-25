using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    public class DriverPortalController : Controller
    {
        private readonly AppDbContext _context;

        public DriverPortalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DriverPortal/Login
        public async Task<IActionResult> Login()
        {
            ViewBag.Trucks = new SelectList(await _context.Trucks.ToListAsync(), "Id", "LicensePlate");
            return View();
        }

        // POST: DriverPortal/Login
        [HttpPost]
        public async Task<IActionResult> Login(int truckId)
        {
            var truck = await _context.Trucks.FindAsync(truckId);
            if (truck == null)
            {
                ModelState.AddModelError("", "Invalid Truck Selection");
                ViewBag.Trucks = new SelectList(await _context.Trucks.ToListAsync(), "Id", "LicensePlate");
                return View();
            }

            // Save in session
            HttpContext.Session.SetInt32("DriverTruckId", truckId);
            HttpContext.Session.SetString("DriverLicensePlate", truck.LicensePlate);

            return RedirectToAction(nameof(Index));
        }

        // GET: DriverPortal/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: DriverPortal/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var truckId = HttpContext.Session.GetInt32("DriverTruckId");
            if (truckId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            ViewBag.LicensePlate = HttpContext.Session.GetString("DriverLicensePlate");
            
            // Find Active Trip for this truck (most recent one that hasn't ended, or just the most recent one for simplicity)
            var activeTrip = await _context.TripRecords
                .Where(t => t.TruckId == truckId)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefaultAsync();

            return View(activeTrip);
        }
    }
}
