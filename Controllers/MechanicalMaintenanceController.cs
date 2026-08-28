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
        public async Task<IActionResult> Create()
        {
            // FIXED: Redirect to truck creation instead of silently creating a dummy
            if (!await _context.Trucks.AnyAsync())
            {
                TempData["Warning"] = "Please add a truck first before logging maintenance.";
                return RedirectToAction("Create", "Trucks");
            }

            ViewBag.Trucks = await _context.Trucks.ToListAsync();
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
            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            return View(record);
        }

        // GET: MechanicalMaintenance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.MechanicalMaintenanceRecords.FindAsync(id);
            if (record == null) return NotFound();

            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            return View(record);
        }

        // POST: MechanicalMaintenance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MechanicalMaintenanceRecord record)
        {
            if (id != record.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.MechanicalMaintenanceRecords.AnyAsync(m => m.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            return View(record);
        }

        // GET: MechanicalMaintenance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.MechanicalMaintenanceRecords
                .Include(m => m.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (record == null) return NotFound();

            return View(record);
        }

        // POST: MechanicalMaintenance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.MechanicalMaintenanceRecords.FindAsync(id);
            if (record != null)
            {
                _context.MechanicalMaintenanceRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
