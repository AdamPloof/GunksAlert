using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DB connection
builder.Services.AddDbContext<GunksDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("GunksAlert"))
);

builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<GunksDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<OpenWeatherBridge, OpenWeatherBridge>();
builder.Services.AddScoped<ForecastManager, ForecastManager>();
builder.Services.AddScoped<WeatherHistoryManager, WeatherHistoryManager>();
builder.Services.AddScoped<WeatherManager, WeatherManager>();
builder.Services.AddScoped<ConditionsChecker, ConditionsChecker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    using (AsyncServiceScope scope = app.Services.CreateAsyncScope()) {
        IServiceProvider services = scope.ServiceProvider;
        string conditionsPath = Path.Combine(
            AppContext.BaseDirectory,
            "var",
            "weatherConditions.json"
        );
        await DailyConditionSeeder.SeedAsync(
            conditionsPath,
            new GunksDbContext(services.GetRequiredService<DbContextOptions<GunksDbContext>>()),
            app.Logger
        );
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
