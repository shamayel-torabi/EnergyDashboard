using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer.Models;

namespace IdentityServer.Data
{
    public class IdentityDbContextSeed
    {
        public async Task SeedAsync(IdentityDbContext context, IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<IdentityDbContextSeed>>();

            try
            {
                using (context)
                {
                    await context.Database.MigrateAsync();

                    var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await EnsureSeedUserData(roleMgr, userMgr, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private static async Task EnsureSeedUserData(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMgr, ILogger<IdentityDbContextSeed> logger)
        {
            string[] roles = { "Admins", "Editors", "Users" };
            foreach (var role in roles)
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    await roleMgr.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userMgr.FindByNameAsync("admin@hrec.co.ir");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@hrec.co.ir",
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@hrec.co.ir",
                    EmailConfirmed = true,
                };
                var result = await userMgr.CreateAsync(admin, "Admin@1234");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRoleAsync(admin, "Admins");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                logger.LogInformation("admin created");
            }
            else
            {
                logger.LogInformation("admin already exists");
            }

            var editor = await userMgr.FindByNameAsync("editor@hrec.co.ir");
            if (editor == null)
            {
                editor = new ApplicationUser
                {
                    UserName = "editor@hrec.co.ir",
                    FirstName = "Editor",
                    LastName = "User",
                    Email = "editor@hrec.co.ir",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(editor, "Editor@1234");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRoleAsync(editor, "Editors");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                logger.LogInformation("editor created");
            }
            else
            {
                logger.LogInformation("editor already exists");
            }


            var user = await userMgr.FindByNameAsync("user@hrec.co.ir");
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "user@hrec.co.ir",
                    FirstName = "Normal",
                    LastName = "User",
                    Email = "user@hrec.co.ir",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(user, "User@1234");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRoleAsync(user, "Users");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                logger.LogInformation("user created");
            }
            else
            {
                logger.LogInformation("user already exists");
            }
        }
    }
}
