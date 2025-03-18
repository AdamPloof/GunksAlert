using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Models;
using GunksAlert.Api.Security;

namespace GunksAlert.Api.Data;

public class GunksDbContext : IdentityDbContext<AppUser> {
    public GunksDbContext(
        DbContextOptions<GunksDbContext> options
    ) : base(options) {}

    public DbSet<Crag> Crags { get; set; } = null!;
    public DbSet<Forecast> Forecasts { get; set; } = null!;
    public DbSet<WeatherHistory> WeatherHistories { get; set; } = null!;
    public DbSet<DailyCondition> DailyConditions { get; set; } = null!;
    public DbSet<ClimbableConditions> ClimbableConditions { get; set; } = null!;
    public DbSet<Alert> Alerts { get; set; } = null!;
    public DbSet<AlertPeriod> AlertPeriods { get; set; } = null!;
    public DbSet<AlertCriteria> AlertCriterias { get; set; } = null!;
    public DbSet<ConditionsReport> ConditionsReports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Crag>().ToTable("Crag");
        modelBuilder.Entity<Forecast>().ToTable("Forecast");
        modelBuilder.Entity<WeatherHistory>().ToTable("WeatherHistory");
        modelBuilder.Entity<DailyCondition>().ToTable("DailyCondition");
        modelBuilder.Entity<ClimbableConditions>().ToTable("ClimbableConditions");
        modelBuilder.Entity<Alert>().ToTable("Alert");
        modelBuilder.Entity<AlertPeriod>().ToTable("AlertPeriod");
        modelBuilder.Entity<AlertCriteria>().ToTable("AlertCriteria");
        modelBuilder.Entity<ConditionsReport>().ToTable("ConditionsReport");
    }
}
