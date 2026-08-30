using System;
using System.Linq;
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
    public class ServiceRemindersController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceRemindersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ServiceReminders
        public async Task<IActionResult> Index(string? status, string? priority, int? truckId, string? search)
        {
            var query = _context.ServiceReminders
                .Include(s => s.Truck)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(s => s.ServiceType.ToLower().Contains(term) ||
                                         (s.Truck != null && s.Truck.LicensePlate.ToLower().Contains(term)) ||
                                         (s.Notes != null && s.Notes.ToLower().Contains(term)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(s => s.Priority == priority);
            }

            if (truckId.HasValue)
            {
                query = query.Where(s => s.TruckId == truckId.Value);
            }

            var list = await query
                .OrderByDescending(s => s.Priority == "Critical")
                .ThenByDescending(s => s.Priority == "High")
                .ThenBy(s => s.DueDate)
                .ToListAsync();

            // Calculate auto-status updates for overdue/due soon
            var today = DateTime.Today;
            foreach (var item in list.Where(s => s.Status != "Completed"))
            {
                bool isOdoOverdue = item.DueOdometer.HasValue && item.Truck != null && item.Truck.CurrentOdometer >= item.DueOdometer.Value;
                bool isDateOverdue = item.DueDate.HasValue && item.DueDate.Value.Date < today;

                if (isOdoOverdue || isDateOverdue)
                {
                    item.Status = "Overdue";
                }
                else if ((item.DueDate.HasValue && item.DueDate.Value.Date <= today.AddDays(7)) ||
                         (item.DueOdometer.HasValue && item.Truck != null && item.Truck.CurrentOdometer >= (item.DueOdometer.Value - 500)))
                {
                    item.Status = "Due Soon";
                }
            }

            // Summary Stats
            ViewBag.TotalCount = await _context.ServiceReminders.CountAsync();
            ViewBag.OverdueCount = list.Count(s => s.Status == "Overdue");
            ViewBag.DueSoonCount = list.Count(s => s.Status == "Due Soon");
            ViewBag.CompletedCount = await _context.ServiceReminders.CountAsync(s => s.Status == "Completed");

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPriority = priority;
            ViewBag.CurrentTruckId = truckId;
            ViewBag.Search = search;
            ViewBag.TrucksList = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate");

            return View(list);
        }

        // GET: ServiceReminders/Create
        public async Task<IActionResult> Create(int? truckId)
        {
            var trucks = await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync();
            ViewBag.TruckId = new SelectList(trucks, "Id", "LicensePlate", truckId);
            ViewBag.TruckDataJson = System.Text.Json.JsonSerializer.Serialize(
                trucks.Select(t => new { t.Id, t.CurrentOdometer, t.LicensePlate, MakeModel = $"{t.Make} {t.Model}" })
            );

            var model = new ServiceReminder
            {
                DueDate = DateTime.Today.AddDays(30),
                Priority = "Medium",
                Status = "Upcoming"
            };

            if (truckId.HasValue)
            {
                model.TruckId = truckId.Value;
                var selectedTruck = trucks.FirstOrDefault(t => t.Id == truckId.Value);
                if (selectedTruck != null)
                {
                    model.LastServicedOdometer = selectedTruck.CurrentOdometer;
                    model.DueOdometer = selectedTruck.CurrentOdometer + 10000;
                }
            }

            return View(model);
        }

        // POST: ServiceReminders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceReminder reminder)
        {
            if (ModelState.IsValid)
            {
                reminder.CreatedAt = DateTime.UtcNow;
                reminder.Status = "Upcoming";

                _context.Add(reminder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Preventive Maintenance Service Reminder scheduled successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", reminder.TruckId);
            return View(reminder);
        }

        // GET: ServiceReminders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reminder = await _context.ServiceReminders.FindAsync(id);
            if (reminder == null) return NotFound();

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", reminder.TruckId);
            return View(reminder);
        }

        // POST: ServiceReminders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceReminder reminder)
        {
            if (id != reminder.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reminder);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Service reminder updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceReminderExists(reminder.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TruckId = new SelectList(await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync(), "Id", "LicensePlate", reminder.TruckId);
            return View(reminder);
        }

        // POST: ServiceReminders/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id, string? completionNotes, bool createNextCycle = true)
        {
            var reminder = await _context.ServiceReminders.Include(s => s.Truck).FirstOrDefaultAsync(s => s.Id == id);
            if (reminder == null) return NotFound();

            reminder.Status = "Completed";
            reminder.CompletedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(completionNotes))
            {
                reminder.Notes = (string.IsNullOrWhiteSpace(reminder.Notes) ? "" : reminder.Notes + " | ") + $"Completed: {completionNotes}";
            }

            // If recurring cycle is set, automatically create the next reminder
            if (createNextCycle && (reminder.IntervalMonths.HasValue || reminder.IntervalKm.HasValue))
            {
                var nextReminder = new ServiceReminder
                {
                    TruckId = reminder.TruckId,
                    ServiceType = reminder.ServiceType,
                    Priority = reminder.Priority,
                    Status = "Upcoming",
                    IntervalMonths = reminder.IntervalMonths,
                    IntervalKm = reminder.IntervalKm,
                    LastServicedDate = DateTime.Today,
                    LastServicedOdometer = reminder.Truck?.CurrentOdometer ?? reminder.DueOdometer,
                    Notes = $"Recurring cycle follow-up for {reminder.ServiceType}",
                    CreatedAt = DateTime.UtcNow
                };

                if (reminder.IntervalMonths.HasValue && reminder.IntervalMonths.Value > 0)
                {
                    nextReminder.DueDate = DateTime.Today.AddMonths(reminder.IntervalMonths.Value);
                }

                if (reminder.IntervalKm.HasValue && reminder.IntervalKm.Value > 0)
                {
                    int baseOdo = reminder.Truck?.CurrentOdometer ?? reminder.DueOdometer ?? 0;
                    nextReminder.DueOdometer = baseOdo + reminder.IntervalKm.Value;
                }

                _context.ServiceReminders.Add(nextReminder);
                TempData["SuccessMessage"] = $"Service marked as Completed! Next recurring service reminder created for {nextReminder.DueDate?.ToShortDateString() ?? "next interval"}.";
            }
            else
            {
                TempData["SuccessMessage"] = "Service reminder marked as Completed!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: ServiceReminders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await _context.ServiceReminders.FindAsync(id);
            if (reminder != null)
            {
                _context.ServiceReminders.Remove(reminder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Service reminder deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceReminderExists(int id)
        {
            return _context.ServiceReminders.Any(e => e.Id == id);
        }
    }
}
