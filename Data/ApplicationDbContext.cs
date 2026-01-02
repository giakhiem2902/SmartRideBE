using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Models;

namespace SmartRideBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BusCompany> BusCompanies { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusSeat> BusSeats { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketSeat> TicketSeats { get; set; }
        public DbSet<Province> Provinces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.BusCompany)
                .WithMany(c => c.Buses)
                .HasForeignKey(b => b.BusCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusSeat>()
                .HasOne(bs => bs.Bus)
                .WithMany(b => b.BusSeats)
                .HasForeignKey(bs => bs.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Bus)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.BusCompany)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.BusCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Trip)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketSeat>()
                .HasOne(ts => ts.Ticket)
                .WithMany(t => t.TicketSeats)
                .HasForeignKey(ts => ts.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketSeat>()
                .HasOne(ts => ts.BusSeat)
                .WithMany(bs => bs.TicketSeats)
                .HasForeignKey(ts => ts.BusSeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            modelBuilder.Entity<Trip>()
                .Property(t => t.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.TotalPrice)
                .HasPrecision(10, 2);
        }
    }
}
