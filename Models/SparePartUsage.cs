using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class SparePartUsage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Spare Part")]
        public int SparePartId { get; set; }

        [ForeignKey("SparePartId")]
        public SparePart? SparePart { get; set; }

        [Required]
        [Display(Name = "Truck")]
        public int TruckId { get; set; }

        [ForeignKey("TruckId")]
        public Truck? Truck { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Quantity used must be at least 1")]
        [Display(Name = "Quantity Used")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Display(Name = "Unit Cost (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "Total Cost (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Card / Reason")]
        public string? JobReference { get; set; }

        [StringLength(100)]
        [Display(Name = "Issued By / Mechanic")]
        public string? IssuedBy { get; set; }

        [Display(Name = "Usage Date")]
        [DataType(DataType.Date)]
        public DateTime UsageDate { get; set; } = DateTime.Today;

        [Display(Name = "Logged At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
