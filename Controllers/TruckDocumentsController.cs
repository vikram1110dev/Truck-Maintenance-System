using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using Truck_Maintanance_system.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class TruckDocumentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public TruckDocumentsController(AppDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
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
            ViewBag.AllTrucks = await _context.Trucks.ToListAsync();
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
                try
                {
                    if (documentFile != null && documentFile.Length > 0)
                    {
                        model.AttachmentPath = await _fileUploadService.SaveFileAsync(
                            documentFile, $"documents/{model.TruckId}");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.TruckId = model.TruckId;
                    return View(model);
                }

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { truckId = model.TruckId });
            }
            ViewBag.TruckId = model.TruckId;
            return View(model);
        }

        // GET: TruckDocuments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doc = await _context.TruckDocuments.Include(d => d.Truck).FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null) return NotFound();

            return View(doc);
        }

        // POST: TruckDocuments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TruckDocument model, IFormFile? documentFile)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new file is uploaded, replace the old one
                    if (documentFile != null && documentFile.Length > 0)
                    {
                        // Delete old file
                        if (!string.IsNullOrEmpty(model.AttachmentPath))
                        {
                            _fileUploadService.DeleteFile(model.AttachmentPath);
                        }
                        model.AttachmentPath = await _fileUploadService.SaveFileAsync(
                            documentFile, $"documents/{model.TruckId}");
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.TruckDocuments.AnyAsync(d => d.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index), new { truckId = model.TruckId });
            }
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
                _fileUploadService.DeleteFile(truckDocument.AttachmentPath ?? "");

                _context.TruckDocuments.Remove(truckDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { truckId = truckId });
            }
            return RedirectToAction("Index", "Trucks");
        }
    }
}
