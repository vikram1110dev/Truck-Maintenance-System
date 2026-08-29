using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public enum TyreStatus
    {
        [Display(Name = "Mounted on Truck")]
        Mounted,
        [Display(Name = "Spare / In Stock")]
        Spare,
        [Display(Name = "Needs Retreading")]
        NeedsRetreading,
        [Display(Name = "Scrapped / Disposed")]
        Scrapped
    }

    public class TyreInventory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tyre Serial Number / DOT")]
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Brand / Manufacturer")]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tyre Size / Specification")]
        [StringLength(50)]
        public string Size { get; set; } = "295/80R22.5";

        [Display(Name = "Assigned Truck")]
        public int? TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Required]
        [Display(Name = "Axle Position")]
        [StringLength(50)]
        public string AxlePosition { get; set; } = "Front-Left (FL)";

        [Required]
        [Display(Name = "Tread Depth (mm)")]
        [Range(0.0, 35.0, ErrorMessage = "Tread depth must be between 0 and 35 mm")]
        public decimal TreadDepthMm { get; set; } = 15.0m;

        [Required]
        [Display(Name = "Condition / Lifecycle Status")]
        public TyreStatus Status { get; set; } = TyreStatus.Mounted;

        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; } = DateTime.Today;

        [Display(Name = "Purchase Cost (₹)")]
        [Range(0, 1000000)]
        public decimal PurchaseCost { get; set; }

        [Display(Name = "Installation Odometer (km)")]
        public int? InstallationOdometer { get; set; }

        [Display(Name = "Notes / Inspection Remarks")]
        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsLowTread => TreadDepthMm <= 4.0m && Status == TyreStatus.Mounted;
    }
}
