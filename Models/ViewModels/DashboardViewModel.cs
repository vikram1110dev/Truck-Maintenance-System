using System;
using System.Collections.Generic;

namespace Truck_Maintanance_system.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<MaintenanceAlert> MaintenanceAlerts { get; set; } = new List<MaintenanceAlert>();
        public List<DocumentAlert> DocumentAlerts { get; set; } = new List<DocumentAlert>();

        // Summary Cards
        public int TotalTrucks { get; set; }
        public int ActiveTrucks { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int OpenAlertCount { get; set; }
        public int MonthlyTripsCount { get; set; }
    }

    public class MaintenanceAlert
    {
        public int TruckId { get; set; }
        public string TruckIdentifier { get; set; } = string.Empty; // e.g. "Volvo FH16 (KA01AB1234)"
        public int LastServiceOdometer { get; set; }
        public int NextServiceDueOdometer { get; set; }
        public int CurrentOdometer { get; set; }
        
        public int KmRemaining => NextServiceDueOdometer - CurrentOdometer;
        
        // "Overdue", "Due Soon"
        public string Status => KmRemaining <= 0 ? "Overdue" : "Due Soon";
    }

    public class DocumentAlert
    {
        public int TruckId { get; set; }
        public string TruckIdentifier { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        
        public int DaysRemaining => (ExpiryDate.Date - DateTime.Today).Days;
        
        // "Expired", "Expiring Soon"
        public string Status => DaysRemaining <= 0 ? "Expired" : "Expiring Soon";
    }
}
