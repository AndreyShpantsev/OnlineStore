using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.HelperServices;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    [Authorize(Roles = "moderator")]
    public class ModeratorController : Controller
    {
        private readonly IPagination<Antiques> paginationAntiquesLogic;
        private readonly ICrudLogic<Antiques> crudAntiquesLogic;
        private readonly ICrudLogic<Note> crudNoteLogic;
        private readonly IBot telegramBot;

        public ModeratorController(IPagination<Antiques> paginationAntiquesLogic,
            ICrudLogic<Antiques> crudAntiquesLogic, ICrudLogic<Note> crudNoteLogic,
            IBot telegramBot)
        {
            this.paginationAntiquesLogic = paginationAntiquesLogic;
            this.crudAntiquesLogic = crudAntiquesLogic;
            this.crudNoteLogic = crudNoteLogic;
            this.telegramBot = telegramBot;
        }

        [HttpGet]
        public async Task<IActionResult> Antiques(int page = 1)
        {
            List<Antiques> antiquesOnModeration = await paginationAntiquesLogic.GetPage(page, new Antiques
            {
                Status = AntiquesStatusProvider.GetOnModerationStatus()
            });

            int antiquesCount = await paginationAntiquesLogic.GetCount(new Antiques
            {
                Status = AntiquesStatusProvider.GetOnModerationStatus()
            });

            return View(new AntiquesViewModel
            {
                Antiques = antiquesOnModeration,
                PageViewModel = new PageViewModel(antiquesCount, page, ApplicationConstantsProvider.GetPageSize())
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckAntiques(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                Antiques antiquesToCheck = (await crudAntiquesLogic.Read(new Antiques
                {
                    Id = id
                }))?.First();
                return View(new AntiquesModerationModel
                {
                    Antiques = antiquesToCheck
                });
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAntiques(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await crudAntiquesLogic.Update(new Antiques
                {
                    Id = id,
                    Status = AntiquesStatusProvider.GetAcceptedStatus()
                });
                await SendAcceptMessage(id);
                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.AntiquesAcceptedMessages(),
                    SecondsToRedirect = ApplicationConstantsProvider.GetLongRedirectionTime(),
                    RedirectUrl = "/Moderator/Antiques"
                });
            }
            return NotFound();
        }

        private async Task SendAcceptMessage(string antiquesId)
        {
            Antiques antiques = (await crudAntiquesLogic.Read(new Antiques
            {
                Id = antiquesId
            })).First();

            if (!string.IsNullOrEmpty(antiques.User.TelegramChatId))
            {
                await telegramBot.SendMessage(
                    $"Ваш антиквариат '{antiques.Name}' успешно прошел модерацию!" +
                    $" Теперь он опубликован на сайте и виден всем пользователям",
                    antiques.User.TelegramChatId);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAntiques(AntiquesModerationModel model)
        {
            if (ModelState.IsValid)
            {
                await crudAntiquesLogic.Update(new Antiques
                {
                    Id = model.Antiques.Id,
                    Status = AntiquesStatusProvider.GetRejectedStatus()
                });
                await crudNoteLogic.Delete(new Note
                {
                    AntiquesId = model.Antiques.Id
                });
                await crudNoteLogic.Create(new Note
                {
                    AntiquesId = model.Antiques.Id,
                    Text = model.RejectNote
                });
                await SendRejectMessage(model.Antiques.Id, model.RejectNote);
                return View("Redirect", new RedirectModel
                {
                    InfoMessages = RedirectionMessageProvider.AntiquesRejectedMessages(),
                    SecondsToRedirect = ApplicationConstantsProvider.GetLongRedirectionTime(),
                    RedirectUrl = "/Moderator/Antiques"
                });
            }
            model.Expanded = true;
            return View("CheckAntiques", model);
        }

        private async Task SendRejectMessage(string antiquesId, string note)
        {
            Antiques antiques = (await crudAntiquesLogic.Read(new Antiques
            {
                Id = antiquesId
            })).First();

            if (!string.IsNullOrEmpty(antiques.User.TelegramChatId))
            {
                await telegramBot.SendMessage(
                    $"Публикация вашего антиквариата '{antiques.Name}' отклонена модератором." +
                    $" Причина, по которой ваш антиквариат не прошел модерацию: {note}",
                    antiques.User.TelegramChatId);
            }
        }
    }
}