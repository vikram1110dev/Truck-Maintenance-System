namespace Truck_Maintanance_system.Models.ViewModels
{
    public class DriverPayrollViewModel
    {
        public List<DriverPayrollStat> DriverStats { get; set; } = new List<DriverPayrollStat>();
        
        public decimal FleetTotalPayroll => DriverStats.Sum(d => d.TotalSalary);
    }

    public class DriverPayrollStat
    {
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        
        public int TotalTrips { get; set; }
        public decimal TotalDistance { get; set; }
        public decimal TotalFuelVolume { get; set; }
        
        public decimal TotalSalary { get; set; }
        
        public decimal AverageEfficiency => TotalFuelVolume > 0 ? Math.Round(TotalDistance / TotalFuelVolume, 2) : 0;
    }
}
