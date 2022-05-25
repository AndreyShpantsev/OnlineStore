using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            IEnumerable<User> users = await userManager.GetUsersInRoleAsync("moderator");
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User moderator = new User
                {
                    Email = model.Email,
                    UserName = model.UserName
                };
                var registerResult = await userManager.CreateAsync(moderator, model.Password);
                if (registerResult.Succeeded)
                {
                    moderator.Email += ApplicationConstantsProvider.AvoidValidationCode();
                    moderator.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                    await userManager.AddToRoleAsync(moderator, "moderator");
                    return RedirectToAction("UsersList");
                }
                else
                {
                    foreach (var error in registerResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}