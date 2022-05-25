using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class AdminInitializer
    {
        public static async Task InitializeAdmin(UserManager<User> userManager, IConfiguration configuration)
        {
            string email = configuration["AdminEmail"];
            string password = configuration["AdminPassword"];
            string username = configuration["AdminUsername"];
            if (await userManager.FindByEmailAsync(email) == null)
            {
                User admin = new User 
                { 
                    Email = email, 
                    UserName = username 
                };
                var registerResult = await userManager.CreateAsync(admin, password);
                if (registerResult.Succeeded)
                {
                    admin.Email += ApplicationConstantsProvider.AvoidValidationCode();
                    admin.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
