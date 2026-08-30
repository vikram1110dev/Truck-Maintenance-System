using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SparePartsController : Controller
    {
        private readonly AppDbContext _context;

        public SparePartsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SpareParts
        public async Task<IActionResult> Index(string? search, string? category, bool? lowStockOnly)
        {
            var query = _context.SpareParts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p => p.PartNumber.ToLower().Contains(term) ||
                                         p.PartName.ToLower().Contains(term) ||
                                         (p.SupplierName != null && p.SupplierName.ToLower().Contains(term)) ||
                                         (p.LocationBin != null && p.LocationBin.ToLower().Contains(term)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (lowStockOnly == true)
            {
                query = query.Where(p => p.QuantityInStock <= p.MinReorderLevel);
            }

            var parts = await query.OrderBy(p => p.Category).ThenBy(p => p.PartName).ToListAsync();

            // KPIs
            var allParts = await _context.SpareParts.ToListAsync();
            ViewBag.TotalPartsCount = allParts.Count;
            ViewBag.TotalStockUnits = allParts.Sum(p => p.QuantityInStock);
            ViewBag.TotalInventoryValue = allParts.Sum(p => p.QuantityInStock * p.UnitCost);
            ViewBag.LowStockCount = allParts.Count(p => p.QuantityInStock <= p.MinReorderLevel && p.QuantityInStock > 0);
            ViewBag.OutOfStockCount = allParts.Count(p => p.QuantityInStock <= 0);

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.LowStockOnly = lowStockOnly;

            ViewBag.TrucksList = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate");

            return View(parts);
        }

        // GET: SpareParts/Create
        public IActionResult Create()
        {
            return View(new SparePart { MinReorderLevel = 3, QuantityInStock = 1 });
        }

        // POST: SpareParts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SparePart sparePart)
        {
            if (ModelState.IsValid)
            {
                sparePart.CreatedAt = DateTime.UtcNow;
                sparePart.LastRestockedDate = DateTime.UtcNow;

                _context.Add(sparePart);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Spare part '{sparePart.PartName}' ({sparePart.PartNumber}) added to inventory.";
                return RedirectToAction(nameof(Index));
            }
            return View(sparePart);
        }

        // GET: SpareParts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sparePart = await _context.SpareParts.FindAsync(id);
            if (sparePart == null) return NotFound();

            return View(sparePart);
        }

        // POST: SpareParts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SparePart sparePart)
        {
            if (id != sparePart.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sparePart);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Spare part '{sparePart.PartName}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SparePartExists(sparePart.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sparePart);
        }

        // POST: SpareParts/Restock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restock(int id, int quantity, decimal? newUnitCost)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Restock quantity must be greater than 0.";
                return RedirectToAction(nameof(Index));
            }

            var part = await _context.SpareParts.FindAsync(id);
            if (part == null) return NotFound();

            part.QuantityInStock += quantity;
            part.LastRestockedDate = DateTime.UtcNow;
            if (newUnitCost.HasValue && newUnitCost.Value > 0)
            {
                part.UnitCost = newUnitCost.Value;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Successfully restocked {quantity} units of '{part.PartName}'. Current stock: {part.QuantityInStock}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: SpareParts/Issue/5
        public async Task<IActionResult> Issue(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.SpareParts.FindAsync(id);
            if (part == null) return NotFound();

            ViewBag.Part = part;
            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate");

            var usage = new SparePartUsage
            {
                SparePartId = part.Id,
                UnitCost = part.UnitCost,
                Quantity = 1,
                UsageDate = DateTime.Today
            };

            return View(usage);
        }

        // POST: SpareParts/IssueToTruck
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueToTruck(SparePartUsage usage)
        {
            var part = await _context.SpareParts.FindAsync(usage.SparePartId);
            if (part == null) return NotFound();

            if (usage.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
            }
            else if (usage.Quantity > part.QuantityInStock)
            {
                ModelState.AddModelError("Quantity", $"Insufficient stock! Only {part.QuantityInStock} units available.");
            }

            if (ModelState.IsValid)
            {
                usage.UnitCost = part.UnitCost;
                usage.TotalCost = usage.Quantity * part.UnitCost;
                usage.CreatedAt = DateTime.UtcNow;

                part.QuantityInStock -= usage.Quantity;

                _context.SparePartUsages.Add(usage);
                await _context.SaveChangesAsync();

                var truck = await _context.Trucks.FindAsync(usage.TruckId);
                TempData["SuccessMessage"] = $"Issued {usage.Quantity}x '{part.PartName}' to Truck {truck?.LicensePlate ?? "N/A"} (Total: ₹{usage.TotalCost:N2}).";
                return RedirectToAction(nameof(UsageHistory));
            }

            ViewBag.Part = part;
            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", usage.TruckId);
            return View("Issue", usage);
        }

        // GET: SpareParts/UsageHistory
        public async Task<IActionResult> UsageHistory(int? truckId, int? sparePartId)
        {
            var query = _context.SparePartUsages
                .Include(u => u.SparePart)
                .Include(u => u.Truck)
                .AsQueryable();

            if (truckId.HasValue)
            {
                query = query.Where(u => u.TruckId == truckId.Value);
            }

            if (sparePartId.HasValue)
            {
                query = query.Where(u => u.SparePartId == sparePartId.Value);
            }

            var list = await query.OrderByDescending(u => u.UsageDate).ThenByDescending(u => u.CreatedAt).ToListAsync();

            ViewBag.TotalSpentOnParts = list.Sum(u => u.TotalCost);
            ViewBag.TotalUnitsConsumed = list.Sum(u => u.Quantity);

            ViewBag.TrucksList = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", truckId);
            ViewBag.PartsList = new SelectList(await _context.SpareParts.OrderBy(p => p.PartName).ToListAsync(), "Id", "PartName", sparePartId);

            return View(list);
        }

        // POST: SpareParts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var part = await _context.SpareParts.FindAsync(id);
            if (part != null)
            {
                _context.SpareParts.Remove(part);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Spare part removed from inventory.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: SpareParts/ExportCsv
        public async Task<IActionResult> ExportCsv()
        {
            var parts = await _context.SpareParts.OrderBy(p => p.Category).ThenBy(p => p.PartName).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Part Number,Part Name,Category,Unit Cost (INR),Quantity in Stock,Min Reorder Level,Location Bin,Supplier,Total Value (INR),Status");

            foreach (var p in parts)
            {
                var status = p.QuantityInStock <= 0 ? "Out of Stock" : p.QuantityInStock <= p.MinReorderLevel ? "Low Stock" : "Adequate";
                sb.AppendLine($"\"{p.PartNumber}\",\"{p.PartName}\",\"{p.Category}\",{p.UnitCost},{p.QuantityInStock},{p.MinReorderLevel},\"{p.LocationBin}\",\"{p.SupplierName}\",{p.TotalStockValue},\"{status}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Spare_Parts_Inventory_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        private bool SparePartExists(int id)
        {
            return _context.SpareParts.Any(e => e.Id == id);
        }
    }
}
