using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using Truck_Maintanance_system.Services;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FuelLogsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public FuelLogsController(AppDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: FuelLogs
        public async Task<IActionResult> Index(int? truckId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.FuelLogs
                .Include(f => f.Truck)
                .AsQueryable();

            if (truckId.HasValue)
            {
                query = query.Where(f => f.TruckId == truckId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(f => f.FuelDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(f => f.FuelDate <= toDate.Value.Date);
            }

            var logs = await query.OrderByDescending(f => f.FuelDate).ToListAsync();

            // Summary metrics
            ViewBag.TotalSpent = logs.Sum(f => f.TotalAmount);
            ViewBag.TotalLiters = logs.Sum(f => f.Liters);
            ViewBag.AvgPricePerLiter = logs.Any() ? logs.Average(f => f.PricePerLiter) : 0;
            ViewBag.TotalFills = logs.Count;

            ViewBag.Trucks = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            ViewBag.SelectedTruckId = truckId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(logs);
        }

        // GET: FuelLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var fuelLog = await _context.FuelLogs
                .Include(f => f.Truck)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fuelLog == null) return NotFound();

            return View(fuelLog);
        }

        // GET: FuelLogs/Create
        public async Task<IActionResult> Create(int? truckId)
        {
            if (!await _context.Trucks.AnyAsync())
            {
                TempData["Warning"] = "Please add a truck first before creating fuel logs.";
                return RedirectToAction("Create", "Trucks");
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            return View(new FuelLog 
            { 
                FuelDate = DateTime.Today,
                TruckId = truckId ?? 0
            });
        }

        // POST: FuelLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuelLog fuelLog, IFormFile? receiptImage)
        {
            if (ModelState.IsValid)
            {
                fuelLog.TotalAmount = Math.Round(fuelLog.Liters * fuelLog.PricePerLiter, 2);

                if (receiptImage != null && receiptImage.Length > 0)
                {
                    try
                    {
                        fuelLog.ReceiptImagePath = await _fileUploadService.SaveFileAsync(receiptImage, "fuel_receipts");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "File upload failed: " + ex.Message);
                        ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", fuelLog.TruckId);
                        return View(fuelLog);
                    }
                }

                _context.Add(fuelLog);

                // Update truck odometer if fuel entry odometer is greater
                var truck = await _context.Trucks.FindAsync(fuelLog.TruckId);
                if (truck != null && fuelLog.OdometerReading > truck.CurrentOdometer)
                {
                    truck.CurrentOdometer = fuelLog.OdometerReading;
                    _context.Update(truck);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Fuel log entry recorded successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", fuelLog.TruckId);
            return View(fuelLog);
        }

        // GET: FuelLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fuelLog = await _context.FuelLogs
                .Include(f => f.Truck)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fuelLog == null) return NotFound();

            return View(fuelLog);
        }

        // POST: FuelLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fuelLog = await _context.FuelLogs.FindAsync(id);
            if (fuelLog != null)
            {
                if (!string.IsNullOrEmpty(fuelLog.ReceiptImagePath))
                {
                    _fileUploadService.DeleteFile(fuelLog.ReceiptImagePath);
                }

                _context.FuelLogs.Remove(fuelLog);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fuel log removed successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
