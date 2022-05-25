using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class UpdateAccountViewModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]
        [Display(Name = "Новый адрес электронной почты")]
        public string NewEmail { get; set; }

        [Display(Name = "Имя пользователя в Telegram")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Неверный формат")]
        public string NewTelegramUserName { get; set; }
    }
}