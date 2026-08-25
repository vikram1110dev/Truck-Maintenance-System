using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Vin { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z]{2}[ -]?[0-9]{2}[a-zA-Z0-9 -]*$", ErrorMessage = "License Plate must start with 2 letters (state) followed by 2 numbers (district), e.g. TN-01 or TN01")]
        public string LicensePlate { get; set; } = string.Empty;

        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
