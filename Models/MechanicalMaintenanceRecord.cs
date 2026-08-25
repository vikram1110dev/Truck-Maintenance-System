using System;
using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class MechanicalMaintenanceRecord
    {
        public int Id { get; set; }
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }
        
        [Display(Name = "Date Logged")]
        public DateTime DateLogged { get; set; } = DateTime.Now;

        [Display(Name = "Odometer (km)")]
        public int OdometerKm { get; set; }

        // --- OIL ---
        [Display(Name = "Engine Oil")]
        public string EngineOil { get; set; } = string.Empty;
        
        [Display(Name = "Transmission Oil")]
        public string TransmissionOil { get; set; } = string.Empty;
        
        public string Coolant { get; set; } = string.Empty;
        
        [Display(Name = "Crown/Axel Oil")]
        public string CrownAxelOil { get; set; } = string.Empty;
        
        [Display(Name = "Hydraulic Oil")]
        public string HydraulicOil { get; set; } = string.Empty;
        
        [Display(Name = "AdBlue (DEF Oil)")]
        public string AdBlueDefOil { get; set; } = string.Empty;
        
        [Display(Name = "Brake Fluid")]
        public string BrakeFluid { get; set; } = string.Empty;

        // --- TYRES ---
        [Display(Name = "Tyre Age & Condition (%)")]
        public int TyreConditionPercent { get; set; }
        
        [Display(Name = "Wheel Alignment")]
        public string WheelAlignment { get; set; } = string.Empty;
        
        [Display(Name = "Spare Wheel Condition")]
        public string SpareWheelCondition { get; set; } = string.Empty;
        
        [Display(Name = "Tyre Pressure")]
        public string TyrePressure { get; set; } = string.Empty;

        // --- FILTER ---
        [Display(Name = "Air Filter")]
        public string AirFilter { get; set; } = string.Empty;
        
        [Display(Name = "Oil Filter")]
        public string OilFilter { get; set; } = string.Empty;
        
        [Display(Name = "Fuel Filter")]
        public string FuelFilter { get; set; } = string.Empty;
        
        [Display(Name = "AC Cabin Filter")]
        public string AcCabinFilter { get; set; } = string.Empty;
        
        [Display(Name = "Hydraulic Filter")]
        public string HydraulicFilter { get; set; } = string.Empty;
        
        [Display(Name = "Water Separator / Diesel Filter")]
        public string WaterSeparatorDieselFilter { get; set; } = string.Empty;

        // --- BRAKE ---
        [Display(Name = "Brake Shoe/Disc (Front)")]
        public string BrakeShoeDiscFront { get; set; } = string.Empty;
        
        [Display(Name = "Brake Shoe/Disc (Rear)")]
        public string BrakeShoeDiscRear { get; set; } = string.Empty;
        
        [Display(Name = "Brake Rotor/Disc (Front)")]
        public string BrakeRotorDiscFront { get; set; } = string.Empty;
        
        [Display(Name = "Brake Rotor/Disc (Rear)")]
        public string BrakeRotorDiscRear { get; set; } = string.Empty;
        
        [Display(Name = "Air Compressor & Valve")]
        public string AirCompressorAndValve { get; set; } = string.Empty;

        // --- OTHERS ---
        public string Greasing { get; set; } = string.Empty;
        
        [Display(Name = "Clutch Plate Life (%)")]
        public int ClutchPlateLifePercent { get; set; }
        
        [Display(Name = "Clutch Plate Age (Months)")]
        public int ClutchPlateAgeMonths { get; set; }
        
        [Display(Name = "Battery Condition (%)")]
        public int BatteryConditionPercent { get; set; }
        
        [Display(Name = "Battery Age (Months)")]
        public int BatteryAgeMonths { get; set; }
    }
}
