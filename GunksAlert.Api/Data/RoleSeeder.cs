using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using GunksAlert.Api.Security;

namespace GunksAlert.Api.Data;

/// <summary>
/// Make sure the roles required by the app exist in the database. Also ensures that any users
/// defined as admins in the app's config are assign the admin role.
/// </summary>
public static class RoleSeeder {
    public static readonly string AdminRoleName = "Admin";

    public static async Task InitializeRoles(IServiceProvider services) {
        RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(AdminRoleName)) {
            await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
        }

        if (!await roleManager.RoleExistsAsync("User")) {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }

    public static async Task AssignAdmins(IServiceProvider services, List<string> admins) {
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        foreach (string adminName in admins) {
            AppUser? admin = await userManager.FindByNameAsync(adminName);
            if (admin != null && !await userManager.IsInRoleAsync(admin, AdminRoleName)) {
                await userManager.AddToRoleAsync(admin, AdminRoleName);
            }
        }
    }
}
