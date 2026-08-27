using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Driver")]
    public class DriverPortalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> _userManager;

        public DriverPortalController(AppDbContext context, Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            
            // Find Active Trip for this truck
            var activeTrip = await _context.TripRecords
                .Where(t => t.TruckId == truckId)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefaultAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var driverTrips = await _context.TripRecords.Where(t => t.DriverId == user.Id).ToListAsync();
                ViewBag.TotalSalary = driverTrips.Sum(t => t.DriverAllowance);
                ViewBag.TotalTrips = driverTrips.Count;
                ViewBag.TotalDistance = driverTrips.Sum(t => t.DistanceKm);
            }

            return View(activeTrip);
        }
        // GET: DriverPortal/CreateAlert
        public IActionResult CreateAlert()
        {
            var truckId = HttpContext.Session.GetInt32("DriverTruckId");
            if (truckId == null) return RedirectToAction(nameof(Login));

            return View();
        }

        // POST: DriverPortal/CreateAlert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAlert(AlertTicket ticket)
        {
            var truckId = HttpContext.Session.GetInt32("DriverTruckId");
            if (truckId == null) return RedirectToAction(nameof(Login));

            if (ModelState.IsValid)
            {
                ticket.TruckId = truckId.Value; // Force the session truck ID
                ticket.CreatedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                
                // Redirect to a Driver-specific details view
                return RedirectToAction(nameof(AlertDetails), new { id = ticket.Id });
            }
            return View(ticket);
        }

        // GET: DriverPortal/AlertDetails/5
        public async Task<IActionResult> AlertDetails(int id)
        {
            var truckId = HttpContext.Session.GetInt32("DriverTruckId");
            if (truckId == null) return RedirectToAction(nameof(Login));

            var ticket = await _context.AlertTickets
                .Include(t => t.Truck)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(m => m.Id == id && m.TruckId == truckId); // Security: Only their own truck
                
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: DriverPortal/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int ticketId, string? messageText, IFormFile? imageFile, IFormFile? audioFile, [FromServices] IWebHostEnvironment env)
        {
            var truckId = HttpContext.Session.GetInt32("DriverTruckId");
            if (truckId == null) return RedirectToAction(nameof(Login));

            var ticket = await _context.AlertTickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.TruckId == truckId);
            if (ticket == null) return NotFound();

            var message = new AlertMessage
            {
                TicketId = ticketId,
                SenderRole = "Driver", // Forced to Driver
                MessageText = messageText,
                Timestamp = DateTime.Now
            };

            string uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "alerts", ticketId.ToString());
            if ((imageFile != null && imageFile.Length > 0) || (audioFile != null && audioFile.Length > 0))
            {
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                using (var stream = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                message.ImagePath = $"/uploads/alerts/{ticketId}/{uniqueFileName}";
            }

            if (audioFile != null && audioFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_voice.webm";
                using (var stream = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                {
                    await audioFile.CopyToAsync(stream);
                }
                message.AudioPath = $"/uploads/alerts/{ticketId}/{uniqueFileName}";
            }

            if (!string.IsNullOrWhiteSpace(message.MessageText) || message.ImagePath != null || message.AudioPath != null)
            {
                _context.AlertMessages.Add(message);
                ticket.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AlertDetails), new { id = ticketId });
        }
    }
}
