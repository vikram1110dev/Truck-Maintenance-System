using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Truck_Maintanance_system.Models
{
    public class ServiceReminder
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a truck")]
        [Display(Name = "Truck")]
        public int TruckId { get; set; }

        [ForeignKey("TruckId")]
        public Truck? Truck { get; set; }

        [Required(ErrorMessage = "Service type is required")]
        [StringLength(100)]
        [Display(Name = "Service Type")]
        public string ServiceType { get; set; } = string.Empty;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Due Odometer (km)")]
        [Range(0, 10000000, ErrorMessage = "Odometer reading must be valid")]
        public int? DueOdometer { get; set; }

        [Display(Name = "Last Serviced Date")]
        [DataType(DataType.Date)]
        public DateTime? LastServicedDate { get; set; }

        [Display(Name = "Last Serviced Odometer (km)")]
        public int? LastServicedOdometer { get; set; }

        [Display(Name = "Recurring Interval (Months)")]
        [Range(0, 120)]
        public int? IntervalMonths { get; set; }

        [Display(Name = "Recurring Interval (km)")]
        [Range(0, 500000)]
        public int? IntervalKm { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Priority")]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Upcoming"; // Upcoming, Due Soon, Overdue, Completed

        [StringLength(500)]
        [Display(Name = "Notes / Instructions")]
        public string? Notes { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }
    }
}
