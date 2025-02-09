using Microsoft.EntityFrameworkCore;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Data;

public class GunksDbContext : DbContext {
    public GunksDbContext(
        DbContextOptions<GunksDbContext> options
    ) : base(options) {}

    public DbSet<Crag> Crags { get; set; } = null!;
    public DbSet<Forecast> Forecasts { get; set; } = null!;
    public DbSet<WeatherHistory> WeatherHistories { get; set; } = null!;
    public DbSet<DailyCondition> DailyConditions { get; set; } = null!;
    public DbSet<ClimbableConditions> ClimbableConditions { get; set; } = null!;
    public DbSet<AlertPeriod> AlertPeriods { get; set; } = null!;
    public DbSet<AlertCriteria> AlertCriterias { get; set; } = null!;
    public DbSet<ClimbabilityReport> ClimbabilityReports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Crag>().ToTable("Crag");
        modelBuilder.Entity<Forecast>().ToTable("Forecast");
        modelBuilder.Entity<WeatherHistory>().ToTable("WeatherHistory");
        modelBuilder.Entity<DailyCondition>().ToTable("DailyCondition");
        modelBuilder.Entity<ClimbableConditions>().ToTable("ClimbableConditions");
        modelBuilder.Entity<AlertPeriod>().ToTable("AlertPeriod");
        modelBuilder.Entity<AlertCriteria>().ToTable("AlertCriteria");
        modelBuilder.Entity<ClimbabilityReport>().ToTable("ClimbabilityReport");
    }
}
