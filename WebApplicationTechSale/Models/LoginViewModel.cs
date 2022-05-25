using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите имя пользователя или Email")]
        [Display(Name = "Имя пользователя или Email")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Не выходить из системы")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}