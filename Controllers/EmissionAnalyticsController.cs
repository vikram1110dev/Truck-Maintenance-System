using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Data;
using Truck_Maintanance_system.Models.ViewModels;

namespace Truck_Maintanance_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmissionAnalyticsController : Controller
    {
        private readonly AppDbContext _context;
        private const double DIESEL_CO2_FACTOR_KG_PER_LITER = 2.68; // DEFRA / EPA Diesel emission factor

        public EmissionAnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EmissionAnalytics
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? truckId)
        {
            var fuelQuery = _context.FuelLogs.Include(f => f.Truck).AsQueryable();
            var tripQuery = _context.TripRecords.Include(t => t.Truck).AsQueryable();

            if (fromDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.FuelDate >= fromDate.Value);
                tripQuery = tripQuery.Where(t => t.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.FuelDate <= toDate.Value);
                tripQuery = tripQuery.Where(t => t.StartDate <= toDate.Value);
            }

            if (truckId.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.TruckId == truckId.Value);
                tripQuery = tripQuery.Where(t => t.TruckId == truckId.Value);
            }

            var fuels = await fuelQuery.ToListAsync();
            var trips = await tripQuery.ToListAsync();
            var trucks = await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync();

            var totalFuelLiters = fuels.Sum(f => f.Liters);
            var totalDistanceKm = trips.Sum(t => t.DistanceKm);
            var totalCo2Kg = (double)totalFuelLiters * DIESEL_CO2_FACTOR_KG_PER_LITER;
            var totalCo2Tonnes = totalCo2Kg / 1000.0;

            double avgIntensityGrams = totalDistanceKm > 0 ? (totalCo2Kg * 1000.0) / (double)totalDistanceKm : 0.0;
            double fleetAvgKmPerL = (totalFuelLiters > 0 && totalDistanceKm > 0) ? (double)totalDistanceKm / (double)totalFuelLiters : 0.0;
            int treesNeeded = (int)Math.Ceiling(totalCo2Tonnes * 45); // ~45 trees offset 1 tonne CO2/year

            string fleetGrade = "A";
            if (avgIntensityGrams > 0)
            {
                if (avgIntensityGrams < 680) fleetGrade = "A+ (Excellent)";
                else if (avgIntensityGrams < 780) fleetGrade = "A (Good)";
                else if (avgIntensityGrams < 880) fleetGrade = "B (Moderate)";
                else if (avgIntensityGrams < 980) fleetGrade = "C (Needs Attention)";
                else fleetGrade = "D (High Emissions)";
            }

            // Group by Truck
            var truckSummaries = new List<TruckEmissionSummary>();
            var activeTruckIds = trucks.Select(t => t.Id).ToList();

            foreach (var truck in trucks)
            {
                if (truckId.HasValue && truck.Id != truckId.Value) continue;

                var truckFuels = fuels.Where(f => f.TruckId == truck.Id).ToList();
                var truckTrips = trips.Where(t => t.TruckId == truck.Id).ToList();

                var liters = truckFuels.Sum(f => f.Liters);
                var distance = truckTrips.Sum(t => t.DistanceKm);
                var co2Kg = (double)liters * DIESEL_CO2_FACTOR_KG_PER_LITER;
                var co2Tonnes = co2Kg / 1000.0;

                double intensity = distance > 0 ? (co2Kg * 1000.0) / (double)distance : 0.0;
                double kmPerL = (liters > 0 && distance > 0) ? (double)distance / (double)liters : 0.0;

                string grade;
                string badgeClass;
                if (intensity > 0)
                {
                    if (intensity < 680) { grade = "A+"; badgeClass = "bg-success text-white"; }
                    else if (intensity < 780) { grade = "A"; badgeClass = "bg-success bg-opacity-75 text-white"; }
                    else if (intensity < 880) { grade = "B"; badgeClass = "bg-info text-dark"; }
                    else if (intensity < 980) { grade = "C"; badgeClass = "bg-warning text-dark"; }
                    else { grade = "D"; badgeClass = "bg-danger text-white"; }
                }
                else
                {
                    grade = "N/A";
                    badgeClass = "bg-secondary text-white";
                }

                truckSummaries.Add(new TruckEmissionSummary
                {
                    TruckId = truck.Id,
                    LicensePlate = truck.LicensePlate,
                    MakeModel = $"{truck.Make} {truck.Model}",
                    TotalFuelLiters = liters,
                    TotalDistanceKm = distance,
                    TotalCo2Kg = Math.Round(co2Kg, 2),
                    TotalCo2Tonnes = Math.Round(co2Tonnes, 3),
                    GramsCo2PerKm = Math.Round(intensity, 1),
                    KmPerLiter = Math.Round(kmPerL, 2),
                    EfficiencyGrade = grade,
                    GradeBadgeClass = badgeClass
                });
            }

            // Monthly Trends
            var monthlyTrends = fuels
                .GroupBy(f => new { Year = f.FuelDate.Year, Month = f.FuelDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g =>
                {
                    var mFuels = g.Sum(f => f.Liters);
                    var mTrips = trips.Where(t => t.StartDate.Year == g.Key.Year && t.StartDate.Month == g.Key.Month).Sum(t => t.DistanceKm);
                    var mCo2Tonnes = ((double)mFuels * DIESEL_CO2_FACTOR_KG_PER_LITER) / 1000.0;
                    var date = new DateTime(g.Key.Year, g.Key.Month, 1);
                    return new MonthlyEmissionTrend
                    {
                        MonthLabel = date.ToString("MMM yyyy"),
                        FuelLiters = mFuels,
                        Co2Tonnes = Math.Round(mCo2Tonnes, 2),
                        DistanceKm = mTrips
                    };
                }).ToList();

            var viewModel = new EmissionAnalyticsViewModel
            {
                TotalFuelLiters = totalFuelLiters,
                TotalDistanceKm = totalDistanceKm,
                TotalCo2EmissionsKg = Math.Round(totalCo2Kg, 2),
                TotalCo2EmissionsTonnes = Math.Round(totalCo2Tonnes, 3),
                AverageEmissionIntensityGramsPerKm = Math.Round(avgIntensityGrams, 1),
                TreesNeededToOffset = treesNeeded,
                FleetEcoRating = fleetGrade,
                FleetAverageKmPerLiter = Math.Round(fleetAvgKmPerL, 2),
                TruckEmissions = truckSummaries.OrderByDescending(t => t.TotalCo2Kg).ToList(),
                MonthlyTrends = monthlyTrends,
                FromDate = fromDate,
                ToDate = toDate,
                SelectedTruckId = truckId
            };

            ViewBag.TrucksList = new SelectList(trucks, "Id", "LicensePlate", truckId);

            return View(viewModel);
        }

        // GET: EmissionAnalytics/ExportEsgReport
        public async Task<IActionResult> ExportEsgReport(DateTime? fromDate, DateTime? toDate)
        {
            var fuelQuery = _context.FuelLogs.Include(f => f.Truck).AsQueryable();
            var tripQuery = _context.TripRecords.Include(t => t.Truck).AsQueryable();

            if (fromDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.FuelDate >= fromDate.Value);
                tripQuery = tripQuery.Where(t => t.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.FuelDate <= toDate.Value);
                tripQuery = tripQuery.Where(t => t.StartDate <= toDate.Value);
            }

            var fuels = await fuelQuery.ToListAsync();
            var trips = await tripQuery.ToListAsync();
            var trucks = await _context.Trucks.OrderBy(t => t.LicensePlate).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Fleet ESG & Carbon Footprint Sustainability Audit Report");
            sb.AppendLine($"Generated On,{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"Reporting Period,{(fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : "All Time")} to {(toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : "Present")}");
            sb.AppendLine($"Emission Standard,DEFRA / EPA Diesel Factor (2.68 kg CO2 per Liter)");
            sb.AppendLine();
            sb.AppendLine("Truck Plate,Make & Model,Total Fuel (L),Distance Traveled (km),Fuel Economy (km/L),Total CO2 (kg),Total CO2 (Tonnes),Emission Intensity (g CO2/km),Eco Grade,Offset Trees Needed");

            foreach (var truck in trucks)
            {
                var truckFuels = fuels.Where(f => f.TruckId == truck.Id).ToList();
                var truckTrips = trips.Where(t => t.TruckId == truck.Id).ToList();

                var liters = truckFuels.Sum(f => f.Liters);
                var distance = truckTrips.Sum(t => t.DistanceKm);
                var co2Kg = (double)liters * DIESEL_CO2_FACTOR_KG_PER_LITER;
                var co2Tonnes = co2Kg / 1000.0;
                double intensity = distance > 0 ? (co2Kg * 1000.0) / (double)distance : 0.0;
                double kmPerL = (liters > 0 && distance > 0) ? (double)distance / (double)liters : 0.0;
                int trees = (int)Math.Ceiling(co2Tonnes * 45);

                string grade = intensity < 680 ? "A+" : intensity < 780 ? "A" : intensity < 880 ? "B" : intensity < 980 ? "C" : "D";

                sb.AppendLine($"\"{truck.LicensePlate}\",\"{truck.Make} {truck.Model}\",{liters},{distance},{kmPerL:N2},{co2Kg:N2},{co2Tonnes:N3},{intensity:N1},\"{grade}\",{trees}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Fleet_Carbon_ESG_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}
