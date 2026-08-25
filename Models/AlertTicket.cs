using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class AlertTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Truck")]
        public int TruckId { get; set; }
        public Truck? Truck { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty; // Breakdown, Accident, Maintenance Request, Document Issue, General

        [Required]
        [Display(Name = "Issue Summary")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Open"; // Open, In Progress, Resolved

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<AlertMessage> Messages { get; set; } = new List<AlertMessage>();
    }
}
