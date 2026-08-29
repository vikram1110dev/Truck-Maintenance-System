using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class FuelLog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Truck")]
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Required]
        [Display(Name = "Driver Name")]
        [StringLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fuel Date")]
        [DataType(DataType.Date)]
        public DateTime FuelDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Fuel Quantity (Liters)")]
        [Range(0.01, 10000.0, ErrorMessage = "Liters must be greater than 0")]
        public decimal Liters { get; set; }

        [Required]
        [Display(Name = "Price Per Liter (₹)")]
        [Range(0.01, 1000.0, ErrorMessage = "Price per liter must be greater than 0")]
        public decimal PricePerLiter { get; set; }

        [Display(Name = "Total Amount (₹)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Odometer Reading (km)")]
        [Range(0, 5000000, ErrorMessage = "Enter valid odometer reading")]
        public int OdometerReading { get; set; }

        [Display(Name = "Fuel Station / Location")]
        [StringLength(150)]
        public string? FuelStation { get; set; }

        [Display(Name = "Receipt Image")]
        public string? ReceiptImagePath { get; set; }

        [Display(Name = "Payment Mode")]
        [StringLength(50)]
        public string PaymentMode { get; set; } = "Cash";

        [Display(Name = "Notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
