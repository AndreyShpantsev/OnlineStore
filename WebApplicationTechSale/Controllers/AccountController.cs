using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IPagination<Antiques> antiquesLogic;
        private readonly ISavedLogic savedListLogic;
        private readonly IBot telegramBot;
        private readonly ICrudLogic<User> userLogic;
        private readonly ICrudLogic<Order> orderLogic;

        public AccountController(IPagination<Antiques> antiquesLogic, ISavedLogic savedListLogic,
            UserManager<User> userManager, SignInManager<User> signInManager, IBot telegramBot,
            ICrudLogic<User> userLogic, ICrudLogic<Order> orderLogic)
        {
            this.antiquesLogic = antiquesLogic;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.savedListLogic = savedListLogic;
            this.telegramBot = telegramBot;
            this.userLogic = userLogic;
            this.orderLogic = orderLogic;
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "Новый и старый пароли не должны совпадать");
                    return View(model);
                }

                User user = await userManager.FindByNameAsync(User.Identity.Name);

                user.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                user.Email += ApplicationConstantsProvider.AvoidValidationCode();

                var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    return View("Redirect", new RedirectModel
                    {
                        InfoMessages = RedirectionMessageProvider.AccountUpdatedMessages(),
                        RedirectUrl = "/Account/Personal",
                        SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Вы ввели неверный старый пароль");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Personal(int page = 1)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            List<Antiques> userAntiques = new List<Antiques>();
            int count = 0;

            if (await userManager.IsInRoleAsync(user, "regular user"))
            {
                userAntiques = await antiquesLogic.GetPage(page, new Antiques
                {
                    User = user
                });
                count = await antiquesLogic.GetCount(new Antiques
                {
                    User = user
                });
            }

            PersonalAccountViewModel model = new PersonalAccountViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                TelegramId = user.TelegramUsername,
                PersonalAntiquesList = new AntiquesViewModel
                {
                    Antiques = userAntiques,
                    PageViewModel = new PageViewModel(count, page, ApplicationConstantsProvider.GetPageSize())
                }
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Antiques", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult loginResult;
                User user = await userManager.FindByEmailAsync(model.Login);
                if (user != null)
                {
                    loginResult = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                }
                else
                {
                    user = await userManager.FindByNameAsync(model.Login);
                    loginResult = await signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                }

                if (loginResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {

                        if (await userManager.IsInRoleAsync(user, "admin"))
                        {
                            return RedirectToAction("UsersList", "Admin");
                        }
                        if (await userManager.IsInRoleAsync(user, "moderator"))
                        {
                            return RedirectToAction("Antiques", "Moderator");
                        }
                        if (await userManager.IsInRoleAsync(user, "regular user"))
                        {
                            return RedirectToAction("Antiques", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Antiques", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Antiques", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.TelegramId))
                {
                    User sameTelegramUser = (await userLogic.Read(new User
                    {
                        TelegramUsername = model.TelegramId
                    }))?.FirstOrDefault();
                    if (sameTelegramUser != null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Уже есть пользователь с таким Telegram-идентификатором");
                        return View(model);
                    }
                }
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    TelegramUsername = string.IsNullOrWhiteSpace(model.TelegramId) ?
                    string.Empty : model.TelegramId,
                    TelegramChatId = string.Empty
                };

                var registerResult = await userManager.CreateAsync(user, model.Password);
                if (registerResult.Succeeded)
                {
                    user.Email += ApplicationConstantsProvider.AvoidValidationCode();
                    user.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                    await userManager.AddToRoleAsync(user, "regular user");
                    await savedListLogic.Create(user);
                    await signInManager.SignInAsync(user, false);
                    return View("Redirect", new RedirectModel
                    {
                        InfoMessages = RedirectionMessageProvider.AccountCreatedMessages(),
                        RedirectUrl = "/Home/Antiques",
                        SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
                    });
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

        [Authorize(Roles = "regular user")]
        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        [Authorize(Roles = "regular user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userToUpdate = await userManager.FindByNameAsync(User.Identity.Name);

                if (!string.IsNullOrWhiteSpace(model.NewEmail))
                {

                    if (model.NewEmail == userToUpdate.Email)
                    {
                        ModelState.AddModelError(string.Empty, "Новый email совпадает со старым");
                        return View(model);
                    }
                    else
                    {
                        userToUpdate.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                        var updateEmailResult = await userManager.SetEmailAsync(userToUpdate, model.NewEmail);
                        if (!updateEmailResult.Succeeded)
                        {
                            foreach (var updateEmailError in updateEmailResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, updateEmailError.Description);
                            }
                            return View(model);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.NewTelegramUserName))
                {
                    if (model.NewTelegramUserName == userToUpdate.TelegramUsername)
                    {
                        ModelState.AddModelError(string.Empty, "Новое имя пользователя в Telegram совпадает со старым");
                        return View(model);
                    }
                    else
                    {
                        User sameTelegramUser = (await userLogic.Read(new User
                        {
                            TelegramUsername = model.NewTelegramUserName
                        }))?.FirstOrDefault();
                        if (sameTelegramUser != null)
                        {
                            ModelState.AddModelError(string.Empty, "Уже есть пользователь с таким Telegram-идентификатором");
                        }
                        else
                        {
                            userToUpdate.Email += ApplicationConstantsProvider.AvoidValidationCode();
                            userToUpdate.UserName += ApplicationConstantsProvider.AvoidValidationCode();
                            userToUpdate.TelegramUsername = model.NewTelegramUserName;
                            string tempChatId = userToUpdate.TelegramChatId;
                            userToUpdate.TelegramChatId = string.Empty;
                            var updateTelegramResult = await userManager.UpdateAsync(userToUpdate);
                            if (updateTelegramResult.Succeeded)
                            {
                                if (!string.IsNullOrWhiteSpace(tempChatId))
                                {
                                    await telegramBot.SendMessage("Вы отписаны от уведомлений, " +
                                    "т.к. изменили учетные данные на сайте", tempChatId);
                                }
                            }
                            else
                            {
                                foreach (var updateTelegramError in updateTelegramResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, updateTelegramError.Description);
                                }
                                return View(model);
                            }
                        }
                    }
                }
                return RedirectToAction("Personal", "Account");
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            List<Order> userOrders = await orderLogic.Read(new Order
            {
                User = new User
                {
                    UserName = user.UserName
                }
            });

            return View(userOrders);
        }
    }
}