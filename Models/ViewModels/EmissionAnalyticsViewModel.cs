using System;
using System.Collections.Generic;

namespace Truck_Maintanance_system.Models.ViewModels
{
    public class TruckEmissionSummary
    {
        public int TruckId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string MakeModel { get; set; } = string.Empty;
        public decimal TotalFuelLiters { get; set; }
        public decimal TotalDistanceKm { get; set; }
        public double TotalCo2Kg { get; set; }
        public double TotalCo2Tonnes { get; set; }
        public double GramsCo2PerKm { get; set; }
        public double KmPerLiter { get; set; }
        public string EfficiencyGrade { get; set; } = "B"; // A+, A, B, C, D
        public string GradeBadgeClass { get; set; } = "bg-success";
    }

    public class MonthlyEmissionTrend
    {
        public string MonthLabel { get; set; } = string.Empty;
        public decimal FuelLiters { get; set; }
        public double Co2Tonnes { get; set; }
        public decimal DistanceKm { get; set; }
    }

    public class EmissionAnalyticsViewModel
    {
        public decimal TotalFuelLiters { get; set; }
        public decimal TotalDistanceKm { get; set; }
        public double TotalCo2EmissionsKg { get; set; }
        public double TotalCo2EmissionsTonnes { get; set; }
        public double AverageEmissionIntensityGramsPerKm { get; set; }
        public int TreesNeededToOffset { get; set; }
        public string FleetEcoRating { get; set; } = "A";
        public double FleetAverageKmPerLiter { get; set; }

        public List<TruckEmissionSummary> TruckEmissions { get; set; } = new();
        public List<MonthlyEmissionTrend> MonthlyTrends { get; set; } = new();

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? SelectedTruckId { get; set; }
    }
}
