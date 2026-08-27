using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class TruckDocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TruckDocumentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: TruckDocuments
        public async Task<IActionResult> Index(int? truckId)
        {
            if (truckId == null)
            {
                // If no truck selected, just get the first one for simplicity, or redirect to a selection page
                var firstTruck = await _context.Trucks.FirstOrDefaultAsync();
                if (firstTruck == null) return RedirectToAction("Index", "Trucks");
                return RedirectToAction(nameof(Index), new { truckId = firstTruck.Id });
            }

            var truck = await _context.Trucks.FindAsync(truckId);
            if (truck == null) return NotFound();

            ViewBag.Truck = truck;
            var documents = await _context.TruckDocuments
                .Where(d => d.TruckId == truckId)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();

            return View(documents);
        }

        // GET: TruckDocuments/Create
        public IActionResult Create(int truckId)
        {
            ViewBag.TruckId = truckId;
            return View();
        }

        // POST: TruckDocuments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TruckDocument model, IFormFile? documentFile)
        {
            if (ModelState.IsValid)
            {
                if (documentFile != null && documentFile.Length > 0)
                {
                    // Create upload directory if it doesn't exist
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents", model.TruckId.ToString());
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Create unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(documentFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await documentFile.CopyToAsync(fileStream);
                    }

                    // Save path to DB
                    model.AttachmentPath = $"/uploads/documents/{model.TruckId}/{uniqueFileName}";
                }

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { truckId = model.TruckId });
            }
            ViewBag.TruckId = model.TruckId;
            return View(model);
        }

        // POST: TruckDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truckDocument = await _context.TruckDocuments.FindAsync(id);
            if (truckDocument != null)
            {
                int truckId = truckDocument.TruckId;
                
                // Delete physical file
                if (!string.IsNullOrEmpty(truckDocument.AttachmentPath))
                {
                    string filePath = Path.Combine(_env.WebRootPath, truckDocument.AttachmentPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.TruckDocuments.Remove(truckDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { truckId = truckId });
            }
            return RedirectToAction("Index", "Trucks");
        }
    }
}
