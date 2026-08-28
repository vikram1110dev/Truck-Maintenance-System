using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class TrucksController : Controller
    {
        private readonly AppDbContext _context;

        public TrucksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trucks
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Trucks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(t =>
                    t.LicensePlate.Contains(search) ||
                    t.Make.Contains(search) ||
                    t.Model.Contains(search) ||
                    t.Vin.Contains(search));
                ViewBag.Search = search;
            }

            return View(await query.OrderBy(t => t.LicensePlate).ToListAsync());
        }

        // GET: Trucks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trucks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Truck truck)
        {
            // Remove the LicensePlate from validation since it's built from the 4 segments
            ModelState.Remove(nameof(Truck.LicensePlate));

            if (ModelState.IsValid)
            {
                // Concatenate the 4 boxes into a single string
                truck.LicensePlate = $"{truck.StateCode.ToUpper()} {truck.RtoCode} {truck.SeriesCode.ToUpper()} {truck.SerialNumber}";
                
                _context.Add(truck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(truck);
        }

        // GET: Trucks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null) return NotFound();

            // Parse the license plate back into segments for editing
            var parts = truck.LicensePlate.Split(' ');
            if (parts.Length >= 4)
            {
                truck.StateCode = parts[0];
                truck.RtoCode = parts[1];
                truck.SeriesCode = parts[2];
                truck.SerialNumber = parts[3];
            }

            return View(truck);
        }

        // POST: Trucks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Truck truck)
        {
            if (id != truck.Id) return NotFound();

            ModelState.Remove(nameof(Truck.LicensePlate));

            if (ModelState.IsValid)
            {
                try
                {
                    truck.LicensePlate = $"{truck.StateCode.ToUpper()} {truck.RtoCode} {truck.SeriesCode.ToUpper()} {truck.SerialNumber}";
                    _context.Update(truck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Trucks.AnyAsync(t => t.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(truck);
        }

        // GET: Trucks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null) return NotFound();

            // Get counts of related records for the warning
            ViewBag.TripCount = await _context.TripRecords.CountAsync(t => t.TruckId == id);
            ViewBag.MaintenanceCount = await _context.MechanicalMaintenanceRecords.CountAsync(m => m.TruckId == id);
            ViewBag.DocumentCount = await _context.TruckDocuments.CountAsync(d => d.TruckId == id);
            ViewBag.AlertCount = await _context.AlertTickets.CountAsync(a => a.TruckId == id);

            return View(truck);
        }

        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truck = await _context.Trucks.FindAsync(id);
            if (truck != null)
            {
                _context.Trucks.Remove(truck);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Trucks/Details/5 — Optimized: single query with Include chains instead of N+1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var truck = await _context.Trucks
                .Include(t => t.Trips)
                .Include(t => t.MaintenanceRecords)
                .Include(t => t.AlertTickets)
                .Include(t => t.Documents)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (truck == null) return NotFound();

            var viewModel = new TruckTimelineViewModel
            {
                Truck = truck,
                TotalTrips = truck.Trips.Count,
                TotalMaintenanceLogs = truck.MaintenanceRecords.Count,
                OpenAlerts = truck.AlertTickets.Count(a => a.Status == "Open")
            };

            // Build timeline events from the already-loaded navigation properties
            foreach (var trip in truck.Trips)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = trip.EndDate,
                    EventType = "Trip",
                    Title = $"Trip: {trip.RouteStart} to {trip.RouteEnd}",
                    Description = $"Freight: ₹{trip.FreightRevenue.ToString("N0")} | Net Profit: ₹{trip.NetTripProfit.ToString("N0")}",
                    IconClass = "fa-route",
                    ColorClass = "border-left-success",
                    Url = $"/Trips"
                });
            }

            foreach (var m in truck.MaintenanceRecords)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = m.DateLogged,
                    EventType = "Maintenance",
                    Title = $"Maintenance Log (Odometer: {m.OdometerKm} km)",
                    Description = $"Total Cost: ₹{m.TotalCost.ToString("N0")}",
                    IconClass = "fa-tools",
                    ColorClass = "border-left-secondary",
                    Url = $"/MechanicalMaintenance/Details/{m.Id}"
                });
            }

            foreach (var a in truck.AlertTickets)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = a.CreatedAt,
                    EventType = "Alert",
                    Title = $"{a.Category}: {a.Title}",
                    Description = $"Status: {a.Status}",
                    IconClass = "fa-exclamation-triangle",
                    ColorClass = a.Status == "Open" ? "border-left-danger" : "border-left-warning",
                    Url = $"/AlertTickets/Details/{a.Id}"
                });
            }

            foreach (var d in truck.Documents)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = d.IssueDate,
                    EventType = "Document",
                    Title = $"{d.DocumentType} Issued/Renewed",
                    Description = $"Expires on {d.ExpiryDate.ToShortDateString()}",
                    IconClass = "fa-file-alt",
                    ColorClass = "border-left-info",
                    Url = $"/TruckDocuments?truckId={id}"
                });
            }

            // Sort all events descending by date
            viewModel.Events = viewModel.Events.OrderByDescending(e => e.EventDate).ToList();

            return View(viewModel);
        }
    }
}
