using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class RedirectModel
    {
        [Required]
        public List<string> InfoMessages { get; set; }
        [Required]
        public int SecondsToRedirect { get; set; }
        [Required]
        public string RedirectUrl { get; set; }
    }
}