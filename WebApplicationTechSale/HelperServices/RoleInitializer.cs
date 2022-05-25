using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class RoleInitializer
    {
        public static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            List<string> roles = new List<string>(configuration["BaseRoles"].Split(','));

            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}