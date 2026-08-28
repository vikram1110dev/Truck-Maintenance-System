using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize]
    public class LiveTrackingController : Controller
    {
        private readonly AppDbContext _context;

        public LiveTrackingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LiveTracking/Driver/5
        // This is the page the driver opens on their mobile phone
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Driver(int? tripId)
        {
            if (tripId == null) return NotFound();

            var trip = await _context.TripRecords.Include(t => t.Truck).FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null) return NotFound();

            return View(trip);
        }

        // POST: API to receive pings from the driver's phone
        [HttpPost]
        [Authorize(Roles = "Admin,Driver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PingLocation(int tripId, double lat, double lng)
        {
            // Validate the trip exists
            var tripExists = await _context.TripRecords.AnyAsync(t => t.Id == tripId);
            if (!tripExists) return NotFound();

            var location = new TripLocation
            {
                TripId = tripId,
                Latitude = lat,
                Longitude = lng,
                Timestamp = DateTime.UtcNow
            };

            _context.TripLocations.Add(location);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // GET: LiveTracking/Track/5
        // This is the page the Owner opens to see the map
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Track(int? tripId)
        {
            if (tripId == null) return NotFound();

            var trip = await _context.TripRecords.Include(t => t.Truck).FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null) return NotFound();

            return View(trip);
        }

        // GET: API to get the latest location for the map to consume
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLatestLocation(int tripId)
        {
            var latest = await _context.TripLocations
                .Where(l => l.TripId == tripId)
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                return NotFound(new { error = "No location data yet" });
            }

            return Json(new 
            { 
                lat = latest.Latitude, 
                lng = latest.Longitude,
                timestamp = latest.Timestamp.ToLocalTime().ToString("g")
            });
        }
    }
}
