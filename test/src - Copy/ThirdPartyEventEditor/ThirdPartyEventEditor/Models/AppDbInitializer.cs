using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ThirdPartyEventEditor.App_Start;

namespace ThirdPartyEventEditor.Models
{
    public class AppDbInitializer
    {
        public static async void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };

            await roleManager.CreateAsync(adminRole);
            await roleManager.CreateAsync(userRole);

            var admin = new ApplicationUser {UserName = ConfigurationManager.AppSettings["AdminEmail"],  Email = ConfigurationManager.AppSettings["AdminEmail"] };
            var result = await userManager.CreateAsync(admin, ConfigurationManager.AppSettings["AdminPassword"]);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin.Id, adminRole.Name);
                await userManager.AddToRoleAsync(admin.Id, userRole.Name);
            }
        }
    }
}