using ATPBCauaLeal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ATPBCauaLeal.Data;

public static class IdentitySeed
{
    public static async Task CriarAdminPadraoAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var contexto = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await contexto.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await userManager.FindByNameAsync("admin");

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Nome = "Administrador"
            };

            await userManager.CreateAsync(admin, "admin123");
        }

        if (!await userManager.IsInRoleAsync(admin, UserRole.Admin.ToString()))
        {
            await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
        }
    }
}
