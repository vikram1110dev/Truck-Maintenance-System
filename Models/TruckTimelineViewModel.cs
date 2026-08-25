using System;
using System.Collections.Generic;

namespace Truck_Maintanance_system.Models
{
    public class TruckTimelineViewModel
    {
        public Truck Truck { get; set; } = null!;
        public int TotalTrips { get; set; }
        public int TotalMaintenanceLogs { get; set; }
        public int OpenAlerts { get; set; }

        public List<TimelineEvent> Events { get; set; } = new List<TimelineEvent>();
    }

    public class TimelineEvent
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty; // "Trip", "Maintenance", "Alert", "Document"
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty; // Link to the specific record
    }
}
