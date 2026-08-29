using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin,Driver")]
    public class VehicleInspectionsController : Controller
    {
        private readonly AppDbContext _context;

        public VehicleInspectionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: VehicleInspections
        public async Task<IActionResult> Index(int? truckId, bool? onlyDefects, InspectionType? type)
        {
            var query = _context.VehicleInspections
                .Include(v => v.Truck)
                .AsQueryable();

            if (truckId.HasValue)
            {
                query = query.Where(v => v.TruckId == truckId.Value);
            }

            if (onlyDefects == true)
            {
                query = query.Where(v => !v.IsSafeToOperate || !v.BrakesOk || !v.LightsAndSignalsOk || !v.TyresAndWheelsOk || !v.EngineOilAndFluidsOk || !v.SteeringAndHornOk || !v.WipersAndGlassOk);
            }

            if (type.HasValue)
            {
                query = query.Where(v => v.Type == type.Value);
            }

            var inspections = await query.OrderByDescending(v => v.InspectionDate).ToListAsync();

            var all = await _context.VehicleInspections.ToListAsync();
            ViewBag.TotalInspections = all.Count;
            ViewBag.PassedCount = all.Count(i => i.IsSafeToOperate && !i.HasDefects);
            ViewBag.DefectCount = all.Count(i => i.HasDefects);
            ViewBag.PreTripCount = all.Count(i => i.Type == InspectionType.PreTrip);

            ViewBag.Trucks = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            ViewBag.SelectedTruckId = truckId;
            ViewBag.OnlyDefects = onlyDefects;
            ViewBag.SelectedType = type;

            return View(inspections);
        }

        // GET: VehicleInspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.VehicleInspections
                .Include(v => v.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // GET: VehicleInspections/Create
        public async Task<IActionResult> Create(int? truckId)
        {
            if (!await _context.Trucks.AnyAsync())
            {
                TempData["Warning"] = "Please add a truck first before submitting an inspection report.";
                return RedirectToAction("Create", "Trucks");
            }

            // If logged in via driver portal session, grab driver info
            var sessionTruckId = HttpContext.Session.GetInt32("DriverTruckId");
            int selectedTruck = truckId ?? sessionTruckId ?? 0;

            var defaultOdometer = 0;
            if (selectedTruck > 0)
            {
                var truck = await _context.Trucks.FindAsync(selectedTruck);
                if (truck != null) defaultOdometer = truck.CurrentOdometer;
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", selectedTruck);

            return View(new VehicleInspectionReport
            {
                TruckId = selectedTruck,
                DriverName = User.Identity?.Name?.Split('@')[0] ?? "Driver",
                InspectionDate = DateTime.Now,
                OdometerReading = defaultOdometer,
                IsSafeToOperate = true,
                BrakesOk = true,
                LightsAndSignalsOk = true,
                TyresAndWheelsOk = true,
                EngineOilAndFluidsOk = true,
                SteeringAndHornOk = true,
                WipersAndGlassOk = true
            });
        }

        // POST: VehicleInspections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleInspectionReport report)
        {
            if (ModelState.IsValid)
            {
                // Auto-set IsSafeToOperate to false if critical defects like brakes or steering fail
                if (!report.BrakesOk || !report.SteeringAndHornOk)
                {
                    report.IsSafeToOperate = false;
                }

                _context.Add(report);

                // Update truck odometer if reported reading is higher
                var truck = await _context.Trucks.FindAsync(report.TruckId);
                if (truck != null && report.OdometerReading > truck.CurrentOdometer)
                {
                    truck.CurrentOdometer = report.OdometerReading;
                    _context.Update(truck);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = report.IsSafeToOperate 
                    ? "Vehicle inspection submitted successfully - PASSED."
                    : "Vehicle inspection submitted - SAFETY DEFECTS FLAGGED FOR MAINTENANCE!";

                if (User.IsInRole("Driver"))
                {
                    return RedirectToAction("Index", "DriverPortal");
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", report.TruckId);
            return View(report);
        }

        // GET: VehicleInspections/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.VehicleInspections
                .Include(v => v.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // POST: VehicleInspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.VehicleInspections.FindAsync(id);
            if (report != null)
            {
                _context.VehicleInspections.Remove(report);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vehicle inspection report removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
