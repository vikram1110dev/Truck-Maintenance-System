using System;
using System.Collections.Generic;

namespace Truck_Maintanance_system.Models.ViewModels
{
    public class FuelAnalyticsViewModel
    {
        public List<TruckFuelStat> TruckStats { get; set; } = new List<TruckFuelStat>();
        
        public decimal FleetTotalDistance { get; set; }
        public decimal FleetTotalFuelVolume { get; set; }
        
        public decimal FleetAverageEfficiency => FleetTotalFuelVolume > 0 
            ? Math.Round(FleetTotalDistance / FleetTotalFuelVolume, 2) 
            : 0;
    }

    public class TruckFuelStat
    {
        public int TruckId { get; set; }
        public string TruckIdentifier { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public decimal TotalDistance { get; set; }
        public decimal TotalFuelVolume { get; set; }
        
        public decimal AverageEfficiency => TotalFuelVolume > 0 
            ? Math.Round(TotalDistance / TotalFuelVolume, 2) 
            : 0;
    }
}
