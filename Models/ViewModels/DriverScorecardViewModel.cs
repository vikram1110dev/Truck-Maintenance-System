using System;
using System.Collections.Generic;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Models.ViewModels
{
    public class DriverScorecardViewModel
    {
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Trip Metrics
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int ActiveTrips { get; set; }
        public double TotalDistanceKm { get; set; }
        public double TripCompletionRate { get; set; }

        // Fuel & Efficiency Metrics
        public decimal TotalFuelLiters { get; set; }
        public decimal TotalFuelCost { get; set; }
        public double KmPerLiter { get; set; }

        // Inspection & DVIR Metrics
        public int TotalInspections { get; set; }
        public int PassedInspections { get; set; }
        public int FailedInspections { get; set; }
        public double InspectionCompliancePercent { get; set; }

        // Safety & Alerts
        public int TotalAlerts { get; set; }
        public int ResolvedAlerts { get; set; }

        // Computed Scoring
        public int SafetyScore { get; set; } // 0 - 100
        public double StarRating { get; set; } // 1.0 - 5.0
        public string PerformanceTier { get; set; } = "Good Standing"; // Elite, Top Performer, Good Standing, Needs Coaching, High Risk
        public string BadgeColorClass { get; set; } = "bg-primary";

        // Lists for details
        public List<TripRecord> RecentTrips { get; set; } = new();
        public List<VehicleInspectionReport> RecentInspections { get; set; } = new();
        public List<FuelLog> RecentFuelLogs { get; set; } = new();
    }

    public class FleetDriverPerformanceIndexViewModel
    {
        public List<DriverScorecardViewModel> Drivers { get; set; } = new();
        public int TotalFleetDrivers { get; set; }
        public double AverageFleetSafetyScore { get; set; }
        public double TotalFleetDistanceKm { get; set; }
        public double AverageFleetFuelEconomy { get; set; }
        public DriverScorecardViewModel? TopSafetyDriver { get; set; }
        public DriverScorecardViewModel? TopMileageDriver { get; set; }
        public DriverScorecardViewModel? MostFuelEfficientDriver { get; set; }
    }
}
