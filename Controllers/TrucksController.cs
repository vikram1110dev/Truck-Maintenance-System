using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    public class TrucksController : Controller
    {
        private readonly AppDbContext _context;

        public TrucksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trucks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trucks.ToListAsync());
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

        // GET: Trucks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null) return NotFound();

            var viewModel = new TruckTimelineViewModel
            {
                Truck = truck
            };

            // 1. Fetch Trips
            var trips = await _context.TripRecords.Where(t => t.TruckId == id).ToListAsync();
            viewModel.TotalTrips = trips.Count;
            foreach (var trip in trips)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = trip.EndDate, // Log trip at end date
                    EventType = "Trip",
                    Title = $"Trip: {trip.RouteStart} to {trip.RouteEnd}",
                    Description = $"Freight: ₹{trip.FreightRevenue.ToString("N0")} | Net Profit: ₹{trip.NetTripProfit.ToString("N0")}",
                    IconClass = "fa-route",
                    ColorClass = "border-left-success",
                    Url = $"/Trips"
                });
            }

            // 2. Fetch Maintenance
            var maintenance = await _context.MechanicalMaintenanceRecords.Where(m => m.TruckId == id).ToListAsync();
            viewModel.TotalMaintenanceLogs = maintenance.Count;
            foreach (var m in maintenance)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = m.DateLogged,
                    EventType = "Maintenance",
                    Title = $"Maintenance Log (Odometer: {m.OdometerKm} km)",
                    Description = "Logged mechanical maintenance.",
                    IconClass = "fa-tools",
                    ColorClass = "border-left-secondary",
                    Url = $"/MechanicalMaintenance/Details/{m.Id}"
                });
            }

            // 3. Fetch Alerts
            var alerts = await _context.AlertTickets.Where(a => a.TruckId == id).ToListAsync();
            viewModel.OpenAlerts = alerts.Count(a => a.Status == "Open");
            foreach (var a in alerts)
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

            // 4. Fetch Documents
            var docs = await _context.TruckDocuments.Where(d => d.TruckId == id).ToListAsync();
            foreach (var d in docs)
            {
                viewModel.Events.Add(new TimelineEvent
                {
                    EventDate = d.IssueDate, // Log at issue date
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
