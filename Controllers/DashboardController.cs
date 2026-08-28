using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models.ViewModels;

namespace Truck_Maintanance_system.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();
            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);

            // --- Summary Cards ---
            viewModel.TotalTrucks = await _context.Trucks.CountAsync();
            viewModel.ActiveTrucks = await _context.Trucks.CountAsync(t => t.Status == Models.TruckStatus.Active);
            viewModel.OpenAlertCount = await _context.AlertTickets.CountAsync(a => a.Status != "Resolved");

            var thisMonthStart = new DateTime(today.Year, today.Month, 1);
            var thisMonthEnd = thisMonthStart.AddMonths(1);
            var monthlyTrips = await _context.TripRecords
                .Where(t => t.EndDate >= thisMonthStart && t.EndDate < thisMonthEnd)
                .ToListAsync();
            viewModel.MonthlyRevenue = monthlyTrips.Sum(t => t.FreightRevenue);
            viewModel.MonthlyTripsCount = monthlyTrips.Count;

            // 1. Get Document Alerts (Expiring in <= 30 days or already expired)
            var documents = await _context.TruckDocuments
                .Include(d => d.Truck)
                .Where(d => d.ExpiryDate <= thirtyDaysFromNow)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();

            foreach (var doc in documents)
            {
                if (doc.Truck != null)
                {
                    viewModel.DocumentAlerts.Add(new DocumentAlert
                    {
                        TruckId = doc.TruckId,
                        TruckIdentifier = $"{doc.Truck.Make} {doc.Truck.Model} ({doc.Truck.LicensePlate})",
                        DocumentId = doc.Id,
                        DocumentType = doc.DocumentType,
                        ExpiryDate = doc.ExpiryDate
                    });
                }
            }

            // 2. Get Maintenance Alerts — FIXED N+1: single query with Join instead of per-truck loop
            var trucksWithLatestMaintenance = await _context.Trucks
                .Select(truck => new
                {
                    Truck = truck,
                    LatestRecord = _context.MechanicalMaintenanceRecords
                        .Where(m => m.TruckId == truck.Id)
                        .OrderByDescending(m => m.DateLogged)
                        .FirstOrDefault()
                })
                .Where(x => x.LatestRecord != null && x.LatestRecord.ValidForNextKm > 0)
                .ToListAsync();

            foreach (var item in trucksWithLatestMaintenance)
            {
                var truck = item.Truck;
                var latestRecord = item.LatestRecord!;

                int nextServiceOdometer = latestRecord.OdometerKm + latestRecord.ValidForNextKm;
                int kmRemaining = nextServiceOdometer - truck.CurrentOdometer;

                // Alert if within 1000 km or overdue
                if (kmRemaining <= 1000)
                {
                    viewModel.MaintenanceAlerts.Add(new MaintenanceAlert
                    {
                        TruckId = truck.Id,
                        TruckIdentifier = $"{truck.Make} {truck.Model} ({truck.LicensePlate})",
                        LastServiceOdometer = latestRecord.OdometerKm,
                        NextServiceDueOdometer = nextServiceOdometer,
                        CurrentOdometer = truck.CurrentOdometer
                    });
                }
            }
            
            // Sort Maintenance alerts so that the most overdue are first
            viewModel.MaintenanceAlerts = viewModel.MaintenanceAlerts
                .OrderBy(m => m.KmRemaining)
                .ToList();

            return View(viewModel);
        }
    }
}
