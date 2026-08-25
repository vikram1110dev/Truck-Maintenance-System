using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Truck_Maintanance_system.Models
{
    [Owned]
    public class MaintenanceItemDetails
    {
        [Display(Name = "Action / Status")]
        public string Status { get; set; } = string.Empty; // Check, Topup, Change, XX%

        [Display(Name = "Valid For Next (km)")]
        public int? ValidForNextKm { get; set; }

        [Display(Name = "Valid For Next (Date)")]
        [DataType(DataType.Date)]
        public DateTime? ValidForNextDate { get; set; }
    }

    public class MechanicalMaintenanceRecord
    {
        public int Id { get; set; }

        [Required]
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Required]
        public DateTime DateLogged { get; set; } = DateTime.Now;

        [Display(Name = "Odometer (km)")]
        public int OdometerKm { get; set; }

        // General overall next valid km (kept for backwards compatibility or general use)
        [Display(Name = "Valid For Next (km)")]
        public int ValidForNextKm { get; set; }

        // --- OIL ---
        [Display(Name = "Engine Oil")]
        public MaintenanceItemDetails EngineOil { get; set; } = new();

        [Display(Name = "Transmission Oil")]
        public MaintenanceItemDetails TransmissionOil { get; set; } = new();

        [Display(Name = "Coolant")]
        public MaintenanceItemDetails Coolant { get; set; } = new();

        [Display(Name = "Crown/Axel Oil")]
        public MaintenanceItemDetails CrownAxelOil { get; set; } = new();

        [Display(Name = "Hydraulic Oil")]
        public MaintenanceItemDetails HydraulicOil { get; set; } = new();

        [Display(Name = "AdBlue (DEF Oil)")]
        public MaintenanceItemDetails AdBlueDefOil { get; set; } = new();

        [Display(Name = "Brake Fluid")]
        public MaintenanceItemDetails BrakeFluid { get; set; } = new();

        // --- TYRES ---
        [Display(Name = "Tyre Age & Condition")]
        public MaintenanceItemDetails TyreCondition { get; set; } = new();

        [Display(Name = "Wheel Alignment")]
        public MaintenanceItemDetails WheelAlignment { get; set; } = new();

        [Display(Name = "Spare Wheel Condition")]
        public MaintenanceItemDetails SpareWheelCondition { get; set; } = new();

        [Display(Name = "Tyre Pressure")]
        public MaintenanceItemDetails TyrePressure { get; set; } = new();

        // --- FILTER ---
        [Display(Name = "Air Filter")]
        public MaintenanceItemDetails AirFilter { get; set; } = new();

        [Display(Name = "Oil Filter")]
        public MaintenanceItemDetails OilFilter { get; set; } = new();

        [Display(Name = "Fuel Filter")]
        public MaintenanceItemDetails FuelFilter { get; set; } = new();

        [Display(Name = "AC Cabin Filter")]
        public MaintenanceItemDetails AcCabinFilter { get; set; } = new();

        [Display(Name = "Hydraulic Filter")]
        public MaintenanceItemDetails HydraulicFilter { get; set; } = new();

        [Display(Name = "Water Separator / Diesel Filter")]
        public MaintenanceItemDetails WaterSeparatorDieselFilter { get; set; } = new();

        // --- BRAKE ---
        [Display(Name = "Brake Shoe/Disc (Front)")]
        public MaintenanceItemDetails BrakeShoeDiscFront { get; set; } = new();

        [Display(Name = "Brake Shoe/Disc (Rear)")]
        public MaintenanceItemDetails BrakeShoeDiscRear { get; set; } = new();

        [Display(Name = "Brake Rotor/Disc (Front)")]
        public MaintenanceItemDetails BrakeRotorDiscFront { get; set; } = new();

        [Display(Name = "Brake Rotor/Disc (Rear)")]
        public MaintenanceItemDetails BrakeRotorDiscRear { get; set; } = new();

        [Display(Name = "Air Compressor and Valve")]
        public MaintenanceItemDetails AirCompressorAndValve { get; set; } = new();

        // --- OTHERS ---
        [Display(Name = "Greasing")]
        public MaintenanceItemDetails Greasing { get; set; } = new();

        [Display(Name = "Clutch Plate Life & Age")]
        public MaintenanceItemDetails ClutchPlateLife { get; set; } = new();

        [Display(Name = "Battery Condition & Age")]
        public MaintenanceItemDetails BatteryCondition { get; set; } = new();
    }
}
