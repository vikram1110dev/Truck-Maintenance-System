using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class SparePart
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Part number / SKU is required")]
        [StringLength(50)]
        [Display(Name = "Part Number / SKU")]
        public string PartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Part name is required")]
        [StringLength(150)]
        [Display(Name = "Part Name")]
        public string PartName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Category")]
        public string Category { get; set; } = "Engine"; // Engine, Braking, Electrical, Transmission, Suspension, Fluids & Oils, Filters, Tyres & Wheels

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Unit cost must be greater than 0")]
        [Display(Name = "Unit Cost (₹)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Stock quantity cannot be negative")]
        [Display(Name = "Quantity in Stock")]
        public int QuantityInStock { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Minimum reorder level cannot be negative")]
        [Display(Name = "Min Reorder Level")]
        public int MinReorderLevel { get; set; } = 3;

        [StringLength(50)]
        [Display(Name = "Bin / Shelf Location")]
        public string? LocationBin { get; set; }

        [StringLength(100)]
        [Display(Name = "Supplier / Vendor")]
        public string? SupplierName { get; set; }

        [StringLength(300)]
        [Display(Name = "Notes / Specifications")]
        public string? Notes { get; set; }

        [Display(Name = "Last Restocked Date")]
        [DataType(DataType.Date)]
        public DateTime? LastRestockedDate { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public bool IsLowStock => QuantityInStock <= MinReorderLevel;

        [NotMapped]
        public bool IsOutOfStock => QuantityInStock <= 0;

        [NotMapped]
        public decimal TotalStockValue => QuantityInStock * UnitCost;

        public ICollection<SparePartUsage> Usages { get; set; } = new List<SparePartUsage>();
    }
}
