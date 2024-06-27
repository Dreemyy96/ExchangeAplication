using CurrencyExchange.Models;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.ViewModels
{
    public class QuestionsVM
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string? Quest { get; set; }

        public IEnumerable<Question>? MainQuestions { get; set; }
    }
}
