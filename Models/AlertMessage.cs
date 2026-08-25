using System;
using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class AlertMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }
        public AlertTicket? Ticket { get; set; }

        [Required]
        public string SenderRole { get; set; } = "Driver"; // "Driver" or "Owner"

        public string? MessageText { get; set; }

        // Path for uploaded images
        public string? ImagePath { get; set; }

        // Path for uploaded voice notes
        public string? AudioPath { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
