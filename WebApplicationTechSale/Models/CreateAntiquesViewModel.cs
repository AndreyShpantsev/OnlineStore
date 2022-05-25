using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using WebApplicationTechSale.HelperServices;

namespace WebApplicationTechSale.Models
{
    public class CreateAntiquesViewModel
    {
        [Display(Name = "Название антиквариата")]
        [Required(ErrorMessage = "Укажите название антиквариата")]
        [MaxLength(100, ErrorMessage = "Не более 100 символов")]
        public string Name { get; set; }

        [Display(Name = "Фотография")]
        [Required(ErrorMessage = "Загрузите фотографию антиквариата")]
        [DataType(DataType.Upload)]
        [ExtensionValidation(new string[]
        { ".jpg", ".jpeg", ".pjpg", ".pjpeg", ".png" }, ErrorMessage = "Неверный формат файла")]
        public IFormFile Photo { get; set; }

        [Display(Name = "Описание антиквариата")]
        [Required(ErrorMessage = "Добавьте описание антиквариата")]
        [MaxLength(500, ErrorMessage = "Не более 500 символов")]
        public string Description { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Укажите цену")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена не должна быть меньше нуля")]
        public int? Price { get; set; }
    }
}