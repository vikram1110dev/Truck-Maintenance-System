using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class AlertTicketsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AlertTicketsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: AlertTickets
        public async Task<IActionResult> Index()
        {
            // Only show active alerts (Open and In Progress)
            var tickets = await _context.AlertTickets
                .Include(t => t.Truck)
                .Where(t => t.Status != "Resolved")
                .OrderByDescending(t => t.Status == "Open")
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
            return View(tickets);
        }

        // GET: AlertTickets/History
        public async Task<IActionResult> History()
        {
            // Show only resolved/cleared alerts
            var tickets = await _context.AlertTickets
                .Include(t => t.Truck)
                .Where(t => t.Status == "Resolved")
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
            return View(tickets);
        }

        // POST: AlertTickets/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear(int id)
        {
            var ticket = await _context.AlertTickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Status = "Resolved";
                ticket.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AlertTickets/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Trucks = await _context.Trucks.ToListAsync();
            return View();
        }

        // POST: AlertTickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlertTicket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreatedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = ticket.Id });
            }
            return View(ticket);
        }

        // GET: AlertTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.AlertTickets
                .Include(t => t.Truck)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: AlertTickets/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int ticketId, string senderRole, string? messageText, IFormFile? imageFile, IFormFile? audioFile)
        {
            var ticket = await _context.AlertTickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var message = new AlertMessage
            {
                TicketId = ticketId,
                SenderRole = senderRole,
                MessageText = messageText,
                Timestamp = DateTime.Now
            };

            // Ensure directory exists
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "alerts", ticketId.ToString());
            if ((imageFile != null && imageFile.Length > 0) || (audioFile != null && audioFile.Length > 0))
            {
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            }

            // Handle Image
            if (imageFile != null && imageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                message.ImagePath = $"/uploads/alerts/{ticketId}/{uniqueFileName}";
            }

            // Handle Audio
            if (audioFile != null && audioFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_voice.webm"; // Or m4a depending on browser
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await audioFile.CopyToAsync(stream);
                }
                message.AudioPath = $"/uploads/alerts/{ticketId}/{uniqueFileName}";
            }

            // Don't save empty messages unless it has media
            if (!string.IsNullOrWhiteSpace(message.MessageText) || message.ImagePath != null || message.AudioPath != null)
            {
                _context.AlertMessages.Add(message);
                
                // Update ticket timestamp
                ticket.UpdatedAt = DateTime.Now;
                _context.Update(ticket);
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }
        
        // POST: AlertTickets/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var ticket = await _context.AlertTickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Status = status;
                ticket.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: AlertTickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.AlertTickets.FindAsync(id);
            if (ticket != null)
            {
                // Delete physical files folder if it exists
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "alerts", id.ToString());
                if (Directory.Exists(uploadsFolder))
                {
                    try
                    {
                        Directory.Delete(uploadsFolder, true);
                    }
                    catch (IOException)
                    {
                        // Log or handle error if directory is locked
                    }
                }

                _context.AlertTickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(History));
        }
    }
}
