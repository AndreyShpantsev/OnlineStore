using DataAccessLogic.DatabaseModels;
using System.Collections.Generic;

namespace WebApplicationTechSale.Models
{
    public class AntiquesViewModel
    {
        public IEnumerable<Antiques> Antiques { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}