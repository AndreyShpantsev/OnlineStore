using System.ComponentModel.DataAnnotations;

namespace WebApplicationTechSale.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неверный формат")]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите имя пользователя")]
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "Слишком короткое имя пользователя")]
        [Display(Name = "Имя пользователя")]
        [RegularExpression("^[а-яА-ЯёЁa-zA-Z0-9]+$", ErrorMessage = "Допустимы только буквы и цифры")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,16}$",
            ErrorMessage = "Ненадежный пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердите пароль")]
        public string PasswordConfirmation { get; set; }
    }
}