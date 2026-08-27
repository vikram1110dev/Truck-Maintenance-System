using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class MechanicalMaintenanceController : Controller
    {
        private readonly AppDbContext _context;

        public MechanicalMaintenanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MechanicalMaintenance (The Board)
        public async Task<IActionResult> Index()
        {
            var records = await _context.MechanicalMaintenanceRecords
                .Include(m => m.Truck)
                .OrderByDescending(m => m.DateLogged)
                .ToListAsync();
            return View(records);
        }

        // GET: MechanicalMaintenance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.MechanicalMaintenanceRecords
                .Include(m => m.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (record == null) return NotFound();

            return View(record);
        }

        // GET: MechanicalMaintenance/Create (The Log Service Form)
        public IActionResult Create()
        {
            // For now, if no trucks exist, create a dummy one so the form works.
            if (!_context.Trucks.Any())
            {
                _context.Trucks.Add(new Truck { Vin = "DUMMY123", LicensePlate = "TN-01-AB-1234", Make = "Tata", Model = "Prima", Year = 2023 });
                _context.SaveChanges();
            }

            ViewBag.Trucks = _context.Trucks.ToList();
            return View(new MechanicalMaintenanceRecord { DateLogged = DateTime.Now });
        }

        // POST: MechanicalMaintenance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MechanicalMaintenanceRecord record)
        {
            if (ModelState.IsValid)
            {
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Trucks = _context.Trucks.ToList();
            return View(record);
        }
    }
}
