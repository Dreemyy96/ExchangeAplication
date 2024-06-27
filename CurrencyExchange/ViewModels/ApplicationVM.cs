using CurrencyExchange.CurrencyEnum;
using CurrencyExchange.Enums;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.ViewModels
{
    public class ApplicationVM
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string? FName { get; set; }

        public AvailableValute CurrencyFrom { get; set; }
        public AvailableValute CurrencyTo { get; set; }

        [Required]
        [Display(Name ="Хочу купить/продать")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [Display(Name = "Дополнительная информация")]
        [DataType(DataType.MultilineText)]
        public string? AdditionalInfo { get; set; } = "";

        [Required]
        public int? BankId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }
    }
}
