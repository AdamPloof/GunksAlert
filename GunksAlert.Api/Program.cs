using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Security;

// The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
// logger configured in `AddSerilog()` below, once configuration and dependency-injection have both been
// set up successfully.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try {
    var builder = WebApplication.CreateBuilder(args);

    // Logging
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(evt =>
                evt.Properties.TryGetValue("SourceContext", out var sourceContext)
                && sourceContext.ToString().Contains("Microsoft.AspNetCore")
            )
            .WriteTo.Console()
        )
        .WriteTo.Logger(lc => lc
            .Filter.ByExcluding(evt =>
                evt.Properties.TryGetValue("SourceContext", out var sourceContext)
                && sourceContext.ToString().Contains("Microsoft.AspNetCore")
            )
            // TODO: change log file name to reflect environment
            .WriteTo.File("./logs/dev_.log", rollingInterval: RollingInterval.Day)
        )
    );

    // Controllers
    builder.Services.AddControllersWithViews();

    // Database
    builder.Services.AddDbContext<GunksDbContext>(options => 
        options.UseNpgsql(builder.Configuration.GetConnectionString("GunksAlert"))
    );

    // Security
    builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
        options.SignIn.RequireConfirmedAccount = false;
    }).AddEntityFrameworkStores<GunksDbContext>();

    builder.Services.AddAuthorization(options => {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    builder.Services.ConfigureApplicationCookie(options => {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
    });

    // General services
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<OpenWeatherBridge, OpenWeatherBridge>();
    builder.Services.AddScoped<ForecastManager, ForecastManager>();
    builder.Services.AddScoped<WeatherHistoryManager, WeatherHistoryManager>();
    builder.Services.AddScoped<WeatherManager, WeatherManager>();
    builder.Services.AddScoped<ConditionsChecker, ConditionsChecker>();
    builder.Services.AddScoped<AlertSender, AlertSender>();
    builder.Services.AddScoped<AlertManager, AlertManager>();
    builder.Services.AddScoped<IAuthenticationProvider, PasswordAuthenticationProvider>();

    var app = builder.Build();

    // Ensure roles are set up
    using (AsyncServiceScope scope = app.Services.CreateAsyncScope()) {
        IServiceProvider services = scope.ServiceProvider;
        await RoleSeeder.InitializeRoles(services);

        IConfiguration config = services.GetRequiredService<IConfiguration>();
        List<string> admins = config.GetSection("AdminUsers").Get<List<string>>()
            ?? throw new ArgumentException("AdminUsers config not set");
        await RoleSeeder.AssignAdmins(services, admins);
    }

    // Seed the database
    if (app.Environment.IsDevelopment()) {
        using (AsyncServiceScope scope = app.Services.CreateAsyncScope()) {
            IServiceProvider services = scope.ServiceProvider;
            string conditionsPath = Path.Combine(
                AppContext.BaseDirectory,
                "var",
                "weatherConditions.json"
            );

            var dbContext = new GunksDbContext(
                services.GetRequiredService<DbContextOptions<GunksDbContext>>()
            );
            await DailyConditionSeeder.SeedAsync(
                conditionsPath,
                dbContext,
                app.Logger
            );
            await ClimbableConditionSeeder.SeedAsync(dbContext);
            await AlertPeriodSeeder.SeedAsync(dbContext);
            await AlertCriteriaSeeder.SeedAsync(dbContext);
        }
    } else {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSerilogRequestLogging();

    #pragma warning disable ASP0014
    app.UseEndpoints((IEndpointRouteBuilder endpoints) => {
        endpoints.MapControllers();
    });
    #pragma warning restore ASP0014

    // app.MapControllerRoute(
    //     name: "default",
    //     pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
    Log.Information("Stopped cleanly");

    return 0;
} catch (Exception e) {
    Log.Fatal(e, "An unhandled exception occurred during bootstrapping");
    return 1;
} finally {
    Log.CloseAndFlush();
}
