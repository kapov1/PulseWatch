using Microsoft.EntityFrameworkCore;
using PulseWatch.Domain;

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
            entity.ToTable("pulsewatch", "public");

            entity.Property(ms => ms.Id).HasColumnName("id");
            entity.Property(ms => ms.Name).HasColumnName("name");
            entity.Property(ms => ms.Url).HasColumnName("url");
        });
    }
}