using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models;
using Truck_Maintanance_system.Models.ViewModels;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriverPerformanceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DriverPerformanceController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DriverPerformance
        public async Task<IActionResult> Index(string? search, string? performanceTier, string? sortBy)
        {
            var driverScorecards = await BuildDriverScorecardsAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var term = search.Trim().ToLower();
                driverScorecards = driverScorecards.Where(d =>
                    d.DriverName.ToLower().Contains(term) ||
                    d.Email.ToLower().Contains(term)).ToList();
            }

            if (!string.IsNullOrEmpty(performanceTier))
            {
                driverScorecards = driverScorecards.Where(d => d.PerformanceTier == performanceTier).ToList();
            }

            // Sorting
            driverScorecards = sortBy switch
            {
                "score_asc" => driverScorecards.OrderBy(d => d.SafetyScore).ToList(),
                "distance_desc" => driverScorecards.OrderByDescending(d => d.TotalDistanceKm).ToList(),
                "trips_desc" => driverScorecards.OrderByDescending(d => d.TotalTrips).ToList(),
                "fuel_desc" => driverScorecards.OrderByDescending(d => d.KmPerLiter).ToList(),
                _ => driverScorecards.OrderByDescending(d => d.SafetyScore).ToList() // Default: highest safety score first
            };

            var viewModel = new FleetDriverPerformanceIndexViewModel
            {
                Drivers = driverScorecards,
                TotalFleetDrivers = driverScorecards.Count,
                AverageFleetSafetyScore = driverScorecards.Any() ? Math.Round(driverScorecards.Average(d => d.SafetyScore), 1) : 0,
                TotalFleetDistanceKm = Math.Round(driverScorecards.Sum(d => d.TotalDistanceKm), 1),
                AverageFleetFuelEconomy = driverScorecards.Where(d => d.KmPerLiter > 0).Any() ?
                    Math.Round(driverScorecards.Where(d => d.KmPerLiter > 0).Average(d => d.KmPerLiter), 2) : 0,
                TopSafetyDriver = driverScorecards.OrderByDescending(d => d.SafetyScore).FirstOrDefault(),
                TopMileageDriver = driverScorecards.OrderByDescending(d => d.TotalDistanceKm).FirstOrDefault(),
                MostFuelEfficientDriver = driverScorecards.Where(d => d.KmPerLiter > 0).OrderByDescending(d => d.KmPerLiter).FirstOrDefault()
            };

            ViewBag.Search = search;
            ViewBag.PerformanceTier = performanceTier;
            ViewBag.SortBy = sortBy;

            return View(viewModel);
        }

        // GET: DriverPerformance/Details?driverName=Ramesh
        public async Task<IActionResult> Details(string? driverName)
        {
            if (string.IsNullOrEmpty(driverName)) return NotFound();

            var driverScorecards = await BuildDriverScorecardsAsync();
            var scorecard = driverScorecards.FirstOrDefault(d => d.DriverName.Equals(driverName, StringComparison.OrdinalIgnoreCase));

            if (scorecard == null)
            {
                TempData["ErrorMessage"] = $"Driver '{driverName}' not found.";
                return RedirectToAction(nameof(Index));
            }

            // Load rich history
            scorecard.RecentTrips = await _context.TripRecords
                .Include(t => t.Truck)
                .Include(t => t.Driver)
                .Where(t => t.Driver != null && (t.Driver.UserName == scorecard.Email || t.Driver.Email == scorecard.Email || (t.Driver.UserName != null && t.Driver.UserName.StartsWith(driverName))))
                .OrderByDescending(t => t.StartDate)
                .Take(10)
                .ToListAsync();

            scorecard.RecentInspections = await _context.VehicleInspections
                .Include(i => i.Truck)
                .Where(i => i.DriverName.ToLower() == driverName.ToLower())
                .OrderByDescending(i => i.InspectionDate)
                .Take(10)
                .ToListAsync();

            scorecard.RecentFuelLogs = await _context.FuelLogs
                .Include(f => f.Truck)
                .Where(f => f.DriverName.ToLower() == driverName.ToLower())
                .OrderByDescending(f => f.FuelDate)
                .Take(10)
                .ToListAsync();

            return View(scorecard);
        }

        // GET: DriverPerformance/ExportCsv
        public async Task<IActionResult> ExportCsv()
        {
            var driverScorecards = await BuildDriverScorecardsAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Driver Name,Email,Total Trips,Completed Trips,Distance (km),Fuel (L),Fuel Cost,Efficiency (km/L),Inspections Done,Compliance (%),Safety Score,Star Rating,Performance Tier");

            foreach (var d in driverScorecards)
            {
                sb.AppendLine($"\"{d.DriverName}\",\"{d.Email}\",{d.TotalTrips},{d.CompletedTrips},{d.TotalDistanceKm},{d.TotalFuelLiters},{d.TotalFuelCost},{d.KmPerLiter},{d.TotalInspections},{d.InspectionCompliancePercent}%,{d.SafetyScore},{d.StarRating},\"{d.PerformanceTier}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Driver_Performance_Scorecards_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        private async Task<List<DriverScorecardViewModel>> BuildDriverScorecardsAsync()
        {
            var driverUsers = await _userManager.GetUsersInRoleAsync("Driver");
            var distinctDriverNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in driverUsers)
            {
                if (!string.IsNullOrEmpty(u.UserName))
                {
                    var nameOnly = u.UserName.Contains("@") ? u.UserName.Split('@')[0] : u.UserName;
                    distinctDriverNames.Add(char.ToUpper(nameOnly[0]) + nameOnly.Substring(1));
                }
            }

            var inspectionDriverNames = await _context.VehicleInspections.Where(i => !string.IsNullOrEmpty(i.DriverName)).Select(i => i.DriverName).Distinct().ToListAsync();
            var fuelDriverNames = await _context.FuelLogs.Where(f => !string.IsNullOrEmpty(f.DriverName)).Select(f => f.DriverName).Distinct().ToListAsync();

            foreach (var name in inspectionDriverNames.Concat(fuelDriverNames))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    distinctDriverNames.Add(name.Trim());
                }
            }

            var allTrips = await _context.TripRecords.Include(t => t.Driver).ToListAsync();
            var allInspections = await _context.VehicleInspections.ToListAsync();
            var allFuelLogs = await _context.FuelLogs.ToListAsync();

            var list = new List<DriverScorecardViewModel>();

            foreach (var name in distinctDriverNames)
            {
                var user = driverUsers.FirstOrDefault(u =>
                    (u.UserName != null && u.UserName.StartsWith(name, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.StartsWith(name, StringComparison.OrdinalIgnoreCase)));

                var driverTrips = allTrips.Where(t =>
                    t.Driver != null && (
                        (t.Driver.UserName != null && t.Driver.UserName.StartsWith(name, StringComparison.OrdinalIgnoreCase)) ||
                        (t.Driver.Email != null && t.Driver.Email.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                    )).ToList();

                var driverInspections = allInspections.Where(i => i.DriverName.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
                var driverFuels = allFuelLogs.Where(f => f.DriverName.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

                var totalTrips = driverTrips.Count;
                var completedTrips = driverTrips.Count(t => t.Status == TripStatus.Completed);
                var activeTrips = driverTrips.Count(t => t.Status == TripStatus.InProgress);
                var totalDistance = (double)driverTrips.Sum(t => t.DistanceKm);
                var tripCompletionRate = totalTrips > 0 ? Math.Round((double)completedTrips / totalTrips * 100.0, 1) : 100.0;

                var totalFuelLiters = driverFuels.Sum(f => f.Liters);
                var totalFuelCost = driverFuels.Sum(f => f.TotalAmount);
                var kmPerLiter = (totalFuelLiters > 0 && totalDistance > 0) ? Math.Round(totalDistance / (double)totalFuelLiters, 2) : 0.0;

                var totalInspections = driverInspections.Count;
                var passedInspections = driverInspections.Count(i => i.IsSafeToOperate);
                var failedInspections = driverInspections.Count(i => !i.IsSafeToOperate);
                var inspectionCompliance = totalInspections > 0 ? Math.Round((double)passedInspections / totalInspections * 100.0, 1) : 100.0;

                // Compute Safety Score (Base 92)
                int score = 92;
                if (totalTrips > 0)
                {
                    if (tripCompletionRate >= 90) score += 4;
                    else score -= 8;
                }
                if (totalInspections > 0)
                {
                    if (inspectionCompliance >= 95) score += 4;
                    else if (inspectionCompliance < 75) score -= 10;
                }
                score -= (failedInspections * 4);

                // Clamp score between 35 and 100
                score = Math.Clamp(score, 35, 100);

                string tier;
                string badgeClass;
                if (score >= 95)
                {
                    tier = "Elite Master";
                    badgeClass = "bg-success text-white";
                }
                else if (score >= 85)
                {
                    tier = "Top Performer";
                    badgeClass = "bg-primary text-white";
                }
                else if (score >= 70)
                {
                    tier = "Good Standing";
                    badgeClass = "bg-info text-dark";
                }
                else if (score >= 55)
                {
                    tier = "Needs Coaching";
                    badgeClass = "bg-warning text-dark font-weight-bold";
                }
                else
                {
                    tier = "High Risk";
                    badgeClass = "bg-danger text-white";
                }

                double stars = Math.Round(score / 20.0, 1);

                list.Add(new DriverScorecardViewModel
                {
                    DriverId = user?.Id ?? name,
                    DriverName = name,
                    Email = user?.Email ?? $"{name.ToLower()}@tms.com",
                    TotalTrips = totalTrips,
                    CompletedTrips = completedTrips,
                    ActiveTrips = activeTrips,
                    TotalDistanceKm = Math.Round(totalDistance, 1),
                    TripCompletionRate = tripCompletionRate,
                    TotalFuelLiters = totalFuelLiters,
                    TotalFuelCost = totalFuelCost,
                    KmPerLiter = kmPerLiter,
                    TotalInspections = totalInspections,
                    PassedInspections = passedInspections,
                    FailedInspections = failedInspections,
                    InspectionCompliancePercent = inspectionCompliance,
                    TotalAlerts = failedInspections,
                    ResolvedAlerts = passedInspections,
                    SafetyScore = score,
                    StarRating = stars,
                    PerformanceTier = tier,
                    BadgeColorClass = badgeClass
                });
            }

            return list;
        }
    }
}
