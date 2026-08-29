using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public enum InspectionType
    {
        [Display(Name = "Pre-Trip Inspection (Before Start)")]
        PreTrip,
        [Display(Name = "Post-Trip Inspection (After Completion)")]
        PostTrip
    }

    public class VehicleInspectionReport
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Required]
        [Display(Name = "Driver Name")]
        [StringLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Inspection Type")]
        public InspectionType Type { get; set; } = InspectionType.PreTrip;

        [Required]
        [Display(Name = "Inspection Date & Time")]
        public DateTime InspectionDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Odometer Reading (km)")]
        [Range(0, 5000000)]
        public int OdometerReading { get; set; }

        // Checklist Items
        [Display(Name = "Brake System & Parking Brake")]
        public bool BrakesOk { get; set; } = true;

        [Display(Name = "Headlights, Tail Lights & Turn Signals")]
        public bool LightsAndSignalsOk { get; set; } = true;

        [Display(Name = "Tyre Pressure & Wheel Lug Nuts")]
        public bool TyresAndWheelsOk { get; set; } = true;

        [Display(Name = "Engine Oil, Coolant & Fluid Levels")]
        public bool EngineOilAndFluidsOk { get; set; } = true;

        [Display(Name = "Steering Mechanism & Horn")]
        public bool SteeringAndHornOk { get; set; } = true;

        [Display(Name = "Windshield, Mirrors & Wipers")]
        public bool WipersAndGlassOk { get; set; } = true;

        [Required]
        [Display(Name = "Is Vehicle Safe to Operate?")]
        public bool IsSafeToOperate { get; set; } = true;

        [Display(Name = "Defects / Issues Reported")]
        [StringLength(1000)]
        public string? DefectsDescription { get; set; }

        [Display(Name = "Corrective Action / Mechanic Notes")]
        [StringLength(1000)]
        public string? CorrectiveActionTaken { get; set; }

        [Display(Name = "General Remarks")]
        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public bool HasDefects => !BrakesOk || !LightsAndSignalsOk || !TyresAndWheelsOk || 
                                  !EngineOilAndFluidsOk || !SteeringAndHornOk || !WipersAndGlassOk || !IsSafeToOperate;
    }
}
