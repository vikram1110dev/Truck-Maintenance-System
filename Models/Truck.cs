using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class Truck
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "VIN (Chassis Number)")]
        public string Vin { get; set; } = string.Empty;

        [Required]
        public string LicensePlate { get; set; } = string.Empty;

        // Form Binding Properties (Not in DB)
        [NotMapped]
        [Required]
        [RegularExpression(@"^[a-zA-Z]{2}$", ErrorMessage = "State code must be 2 letters (e.g., TN)")]
        public string StateCode { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        [RegularExpression(@"^[0-9]{2}$", ErrorMessage = "RTO code must be 2 digits (e.g., 01)")]
        public string RtoCode { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        [RegularExpression(@"^[a-zA-Z]{1,2}$", ErrorMessage = "Series must be 1 or 2 letters (e.g., AB)")]
        public string SeriesCode { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Number must be exactly 4 digits (e.g., 1234)")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        public string Make { get; set; } = string.Empty;
        
        [Required]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        public int Year { get; set; }
    }
}
