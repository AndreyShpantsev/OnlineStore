using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLogic.DatabaseModels
{
    public class Note
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Введите текст сообщения")]
        [MaxLength(250, ErrorMessage = "Слишком длинное сообщение")]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string AntiquesId { get; set; }
        public Antiques Antiques { get; set; }
    }
}