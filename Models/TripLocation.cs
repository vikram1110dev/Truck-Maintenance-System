using System;
using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class TripLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }
        public TripRecord? Trip { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
