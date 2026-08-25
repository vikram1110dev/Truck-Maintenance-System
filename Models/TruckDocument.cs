using System.ComponentModel.DataAnnotations;

namespace Truck_Maintanance_system.Models
{
    public class TruckDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TruckId { get; set; }

        public Truck? Truck { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Document/Policy Number")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Provider/Authority")]
        public string? Provider { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Renewal Expense (₹)")]
        public decimal? Cost { get; set; }

        [Display(Name = "Attachment")]
        public string? AttachmentPath { get; set; }

        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }
    }
}
