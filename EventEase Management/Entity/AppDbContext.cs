using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Entity
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venue>()
              .HasKey(v => v.VenueID);

            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventID);

            modelBuilder.Entity<Event>()
              .HasOne(e => e.Venues)
              .WithMany()
              .HasForeignKey(e => e.VenueId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
            .HasOne(b => b.Venue)
            .WithMany()
            .HasForeignKey(b => b.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Events)
                .WithMany()
                .HasForeignKey(b => b.EventID)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
