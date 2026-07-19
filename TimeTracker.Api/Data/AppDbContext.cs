using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeEntry>()
            .Property(t => t.Hours)
            .HasPrecision(6, 2);

        modelBuilder.Entity<TimeEntry>()
            .Property(t => t.HourlyRateOverride)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Project>()
            .Property(p => p.DefaultHourlyRate)
            .HasPrecision(10, 2);
    }
}