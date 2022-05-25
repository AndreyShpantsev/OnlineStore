using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "regular user")]
    public class UserController : Controller
    {
        private readonly ICrudLogic<Antiques> antiquesLogic;
        private readonly IWebHostEnvironment environment;
        private readonly ISavedLogic savedListLogic;
        private readonly UserManager<User> userManager;
        private readonly IBot telegramBot;
        private readonly ICrudLogic<Order> orderLogic;

        public UserController(ICrudLogic<Antiques> antiquesLogic, IWebHostEnvironment environment,
            UserManager<User> userManager, ISavedLogic savedListLogic, IBot telegramBot, ICrudLogic<Order> orderLogic)
        {
            this.antiquesLogic = antiquesLogic;
            this.environment = environment;
            this.userManager = userManager;
            this.savedListLogic = savedListLogic;
            this.telegramBot = telegramBot;
            this.orderLogic = orderLogic;
        }

        [HttpGet]
        public IActionResult CreateAntiques()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAntiques(CreateAntiquesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Antiques toAdd = new Antiques
                {
                    Name = model.Name,
                    User = new User
                    {
                        UserName = User.Identity.Name
                    },
                    Description = model.Description,
                    Price = (int)model.Price
                };

                string dbPhotoPath = $"/images/{User.Identity.Name}/{model.Name}/photo{Path.GetExtension(model.Photo.FileName)}";
                toAdd.PhotoSrc = dbPhotoPath;

                try
                {
                    await antiquesLogic.Create(toAdd);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }

                string physicalDirectory = Path.GetDirectoryName($"{environment.WebRootPath + dbPhotoPath}");
                if (!Directory.Exists(physicalDirectory))
                {
                    Directory.CreateDirectory(physicalDirectory);
                }

                using (FileStream fs = new FileStream($"{environment.WebRootPath + dbPhotoPath}", FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fs);
                }

                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.AntiquesCreatedMessages(),
                    RedirectUrl = "/Home/Antiques",
                    SecondsToRedirect = ApplicationConstantsProvider.GetMaxRedirectionTime()
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OpenAntiques(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                Antiques antiques = (await antiquesLogic.Read(new Antiques
                {
                    Id = id
                })).First();

                User user = await userManager.FindByNameAsync(User.Identity.Name);

                SavedList userList = await savedListLogic.Read(user);

                if (userList.Antiques.Any(antiques => antiques.Id == id))
                {
                    ViewBag.IsSaved = true;
                }
                else
                {
                    ViewBag.IsSaved = false;
                }

                if (antiques == null)
                {
                    return NotFound();
                }
                return View(antiques);
            }
            return NotFound();
        }

        private async Task SendNotifications(string antiquesId)
        {
            Antiques antiques = (await antiquesLogic.Read(new Antiques
            {
                Id = antiquesId
            })).First();

            if (antiques.User != null 
            && !string.IsNullOrEmpty(antiques.User.TelegramChatId))
            {
                await telegramBot.SendMessage(
                    $"Ваш антиквариат '{antiques.Name}', " + $"продан" +
                    $"по цене {antiques.Price}",
                    antiques.User.TelegramChatId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAntiques(string antiquesId)
        {
            if (!string.IsNullOrWhiteSpace(antiquesId))
            {
                User user = await userManager.FindByNameAsync(User.Identity.Name);
                Antiques antiquesToAdd = new Antiques { Id = antiquesId };
                await savedListLogic.Add(user, antiquesToAdd);
                return RedirectToAction("OpenAntiques", "User", new { id = antiquesId });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> MySavedList()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);

            SavedList userSavedList = await savedListLogic.Read(user);

            return View(userSavedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAntiques(string antiquesId)
        {
            if (!string.IsNullOrWhiteSpace(antiquesId))
            {
                User user = await userManager.FindByNameAsync(User.Identity.Name);
                Antiques antiquesToAdd = new Antiques { Id = antiquesId };
                await savedListLogic.Remove(user, antiquesToAdd);
                return RedirectToAction("OpenAntiques", "User", new { id = antiquesId });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditAntiques(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                Antiques antiquesToEdit = (await antiquesLogic.Read(new Antiques { Id = id })).First();
                if (antiquesToEdit.Status == AntiquesStatusProvider.GetRejectedStatus()
                    || antiquesToEdit.Status == AntiquesStatusProvider.GetAcceptedStatus())
                {
                    if (antiquesToEdit.Status == AntiquesStatusProvider.GetRejectedStatus())
                    {
                        ViewBag.RejectNote = "Причина, по которой ваш антиквариат не был опубликован: "
                            + antiquesToEdit.Note.Text;
                    }
                    else
                    {
                        ViewBag.RejectNote = string.Empty;
                    }
                    return View(new EditAntiquesViewModel
                    {
                        Id = antiquesToEdit.Id,
                        Description = antiquesToEdit.Description,
                        Name = antiquesToEdit.Name,
                        OldName = antiquesToEdit.Name,
                        Price = antiquesToEdit.Price,
                        OldPhotoSrc = antiquesToEdit.PhotoSrc
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAntiques(EditAntiquesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    return NotFound();
                }

                Antiques antiquesToEdit = new Antiques
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Status = AntiquesStatusProvider.GetOnModerationStatus(),
                    Price = (int)model.Price
                };

                string newDbPath = $"/images/{User.Identity.Name}/{model.Name}/photo{Path.GetExtension(model.Photo.FileName)}";
                antiquesToEdit.PhotoSrc = newDbPath;

                try
                {
                    await antiquesLogic.Update(antiquesToEdit);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }

                string oldPath = $"{environment.WebRootPath + Path.GetDirectoryName(model.OldPhotoSrc)}";
                if (Directory.Exists(oldPath))
                {
                    Directory.Delete(oldPath, true);
                }

                string newPhysicalDirectory = Path.GetDirectoryName($"{environment.WebRootPath + newDbPath}");

                if (!Directory.Exists(newPhysicalDirectory))
                {
                    Directory.CreateDirectory(newPhysicalDirectory);
                }

                using (FileStream fs = new FileStream($"{environment.WebRootPath + newDbPath}", FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fs);
                }

                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.AntiquesUpdatedMessages(),
                    RedirectUrl = "/Home/Antiques",
                    SecondsToRedirect = ApplicationConstantsProvider.GetMaxRedirectionTime()
                });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string antiquesId)
        {
            await orderLogic.Create(new Order
            {
                AntiquesId = antiquesId,
                UserName = User.Identity.Name
            });

            await SendNotifications(antiquesId);

            return View("Redirect", new RedirectModel
            {
                InfoMessages = RedirectionMessageProvider.OrderCreateMessage(),
                RedirectUrl = "/Account/MyOrders",
                SecondsToRedirect = ApplicationConstantsProvider.GetShortRedirectionTime()
            });
        }
    }
}