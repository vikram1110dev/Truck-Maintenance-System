using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Truck> Trucks { get; set; } = null!;
        public DbSet<MechanicalMaintenanceRecord> MechanicalMaintenanceRecords { get; set; } = null!;
        public DbSet<TruckDocument> TruckDocuments { get; set; } = null!;
        public DbSet<AlertTicket> AlertTickets { get; set; } = null!;
        public DbSet<AlertMessage> AlertMessages { get; set; } = null!;
        public DbSet<TripRecord> TripRecords { get; set; } = null!;
        public DbSet<TripLocation> TripLocations { get; set; } = null!;
    }
}
