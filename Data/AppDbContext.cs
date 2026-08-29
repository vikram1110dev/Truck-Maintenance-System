using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Truck_Maintanance_system.Models;

namespace Truck_Maintanance_system.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
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
        public DbSet<FuelLog> FuelLogs { get; set; } = null!;
        public DbSet<TyreInventory> TyreInventories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Indexes on foreign keys for query performance ---
            modelBuilder.Entity<TripRecord>().HasIndex(t => t.TruckId);
            modelBuilder.Entity<TripRecord>().HasIndex(t => t.DriverId);
            modelBuilder.Entity<MechanicalMaintenanceRecord>().HasIndex(m => m.TruckId);
            modelBuilder.Entity<TruckDocument>().HasIndex(d => d.TruckId);
            modelBuilder.Entity<AlertTicket>().HasIndex(a => a.TruckId);
            modelBuilder.Entity<TripLocation>().HasIndex(l => l.TripId);
            modelBuilder.Entity<AlertMessage>().HasIndex(m => m.TicketId);
            modelBuilder.Entity<FuelLog>().HasIndex(f => f.TruckId);
            modelBuilder.Entity<FuelLog>().HasIndex(f => f.FuelDate);
            modelBuilder.Entity<TyreInventory>().HasIndex(ty => ty.TruckId);
            modelBuilder.Entity<TyreInventory>().HasIndex(ty => ty.SerialNumber);

            // --- Relationships ---
            modelBuilder.Entity<Truck>()
                .HasMany(t => t.Trips)
                .WithOne(tr => tr.Truck)
                .HasForeignKey(tr => tr.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Truck>()
                .HasMany(t => t.FuelLogs)
                .WithOne(f => f.Truck)
                .HasForeignKey(f => f.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Truck>()
                .HasMany(t => t.Tyres)
                .WithOne(ty => ty.Truck)
                .HasForeignKey(ty => ty.TruckId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Truck>()
                .HasMany(t => t.MaintenanceRecords)
                .WithOne(m => m.Truck)
                .HasForeignKey(m => m.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Truck>()
                .HasMany(t => t.Documents)
                .WithOne(d => d.Truck)
                .HasForeignKey(d => d.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Truck>()
                .HasMany(t => t.AlertTickets)
                .WithOne(a => a.Truck)
                .HasForeignKey(a => a.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlertTicket>()
                .HasMany(a => a.Messages)
                .WithOne(m => m.Ticket)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TripRecord>()
                .HasMany<TripLocation>()
                .WithOne(l => l.Trip)
                .HasForeignKey(l => l.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

