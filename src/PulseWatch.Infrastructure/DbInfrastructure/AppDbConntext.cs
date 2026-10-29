using Microsoft.EntityFrameworkCore;
using PulseWatch.Domain.Models;

namespace PulseWatch.Infrastructure.DbInfrastructure;

public sealed class AppDbContext : DbContext
{
    public DbSet<MonitoredSite> MonitoredSites => Set<MonitoredSite>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MonitoredSite>(entity =>
        {
            entity.ToTable("monitored_sites", "public");

            entity.Property(ms => ms.Id).HasColumnName("id");
            entity.Property(ms => ms.Name).HasColumnName("name");
            entity.Property(ms => ms.Url).HasColumnName("url");
        });
    }
}