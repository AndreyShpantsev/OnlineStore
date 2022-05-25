using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSaleTelegramBot;
using WebApplicationTechSale.Models;

namespace WebApplicationTechSale.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPagination<Antiques> antiquesLogic;

        public HomeController(IPagination<Antiques> antiquesLogic)
        {
            this.antiquesLogic = antiquesLogic;
        }

        [HttpGet]
        public IActionResult Rules()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Antiques(int page = 1)
        {
            List<Antiques> antiquesToDisplay = await antiquesLogic.GetPage(page, new Antiques
            {
                Status = AntiquesStatusProvider.GetAcceptedStatus()
            });

            int antiquesCount = await antiquesLogic.GetCount(new Antiques
            {
                Status = AntiquesStatusProvider.GetAcceptedStatus()
            });

            return View(new AntiquesViewModel()
            {
                PageViewModel = new PageViewModel(antiquesCount, page, ApplicationConstantsProvider.GetPageSize()),
                Antiques = antiquesToDisplay
            });
        }
    }
}