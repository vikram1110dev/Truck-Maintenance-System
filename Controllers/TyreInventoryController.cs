using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TyreInventoryController : Controller
    {
        private readonly AppDbContext _context;

        public TyreInventoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TyreInventory
        public async Task<IActionResult> Index(int? truckId, TyreStatus? status)
        {
            var query = _context.TyreInventories
                .Include(t => t.Truck)
                .AsQueryable();

            if (truckId.HasValue)
            {
                query = query.Where(t => t.TruckId == truckId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            var tyres = await query.OrderBy(t => t.TruckId).ThenBy(t => t.AxlePosition).ToListAsync();

            // Fleet KPI calculations
            var allTyres = await _context.TyreInventories.ToListAsync();
            ViewBag.TotalTyres = allTyres.Count;
            ViewBag.MountedTyres = allTyres.Count(t => t.Status == TyreStatus.Mounted);
            ViewBag.LowTreadWarnings = allTyres.Count(t => t.Status == TyreStatus.Mounted && t.TreadDepthMm <= 4.0m);
            ViewBag.SpareTyres = allTyres.Count(t => t.Status == TyreStatus.Spare);

            ViewBag.Trucks = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            ViewBag.SelectedTruckId = truckId;
            ViewBag.SelectedStatus = status;

            return View(tyres);
        }

        // GET: TyreInventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tyre = await _context.TyreInventories
                .Include(t => t.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tyre == null) return NotFound();

            return View(tyre);
        }

        // GET: TyreInventory/Create
        public async Task<IActionResult> Create(int? truckId)
        {
            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            return View(new TyreInventory
            {
                TruckId = truckId,
                PurchaseDate = DateTime.Today,
                TreadDepthMm = 15.0m,
                AxlePosition = "Front-Left (FL)"
            });
        }

        // POST: TyreInventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TyreInventory tyre)
        {
            if (ModelState.IsValid)
            {
                // If assigned to a truck, populate current installation odometer if available
                if (tyre.TruckId.HasValue && (!tyre.InstallationOdometer.HasValue || tyre.InstallationOdometer == 0))
                {
                    var truck = await _context.Trucks.FindAsync(tyre.TruckId.Value);
                    if (truck != null)
                    {
                        tyre.InstallationOdometer = truck.CurrentOdometer;
                    }
                }

                _context.Add(tyre);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Tyre {tyre.SerialNumber} ({tyre.Brand}) added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", tyre.TruckId);
            return View(tyre);
        }

        // GET: TyreInventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tyre = await _context.TyreInventories.FindAsync(id);
            if (tyre == null) return NotFound();

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", tyre.TruckId);
            return View(tyre);
        }

        // POST: TyreInventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TyreInventory tyre)
        {
            if (id != tyre.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tyre);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Tyre {tyre.SerialNumber} updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.TyreInventories.AnyAsync(e => e.Id == tyre.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", tyre.TruckId);
            return View(tyre);
        }

        // GET: TyreInventory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tyre = await _context.TyreInventories
                .Include(t => t.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tyre == null) return NotFound();

            return View(tyre);
        }

        // POST: TyreInventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tyre = await _context.TyreInventories.FindAsync(id);
            if (tyre != null)
            {
                _context.TyreInventories.Remove(tyre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tyre inventory record deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
