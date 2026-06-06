using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitCore.DAL.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var defaultRoles = new List<Role>
                {
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Trainer" },
                    new Role { RoleName = "Member" }
                };

                await context.Roles.AddRangeAsync(defaultRoles);
                await context.SaveChangesAsync();
            }
        }
    }
}
