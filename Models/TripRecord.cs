using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class TripRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Truck")]
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Display(Name = "Driver")]
        public string? DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Microsoft.AspNetCore.Identity.IdentityUser? Driver { get; set; }

        [Required]
        [Display(Name = "Start Location")]
        public string RouteStart { get; set; } = string.Empty;

        [Required]
        [Display(Name = "End Location")]
        public string RouteEnd { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Freight Revenue (₹)")]
        public decimal FreightRevenue { get; set; }

        [Display(Name = "Fuel Cost (₹)")]
        public decimal FuelCost { get; set; }

        [Display(Name = "Toll Cost (₹)")]
        public decimal TollCost { get; set; }

        [Display(Name = "Driver Allowance (₹)")]
        public decimal DriverAllowance { get; set; }

        [Display(Name = "Other Expenses (₹)")]
        public decimal OtherExpenses { get; set; }

        [Display(Name = "Distance (km)")]
        public decimal DistanceKm { get; set; }

        [Display(Name = "Fuel Volume (Liters)")]
        public decimal FuelVolumeLiters { get; set; }

        [Display(Name = "Trip Notes")]
        public string? Notes { get; set; }

        // Computed Property (Not mapped to DB)
        [NotMapped]
        public decimal NetTripProfit => FreightRevenue - (FuelCost + TollCost + DriverAllowance + OtherExpenses);

        [NotMapped]
        [Display(Name = "Fuel Efficiency (km/l)")]
        public decimal FuelEfficiency => FuelVolumeLiters > 0 ? Math.Round(DistanceKm / FuelVolumeLiters, 2) : 0;
    }
}
