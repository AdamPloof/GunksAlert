using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Security;

var builder = WebApplication.CreateBuilder(args);

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

#pragma warning disable ASP0014
app.UseEndpoints((IEndpointRouteBuilder endpoints) => {
    endpoints.MapControllers();
});
#pragma warning restore ASP0014

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
