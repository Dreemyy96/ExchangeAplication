using CurrencyExchange.CurrencyEnum;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.ViewModels
{
    public class TransferRequestVM
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public AvailableValute Currency { get; set; }

        [Required]
        [StringLength(100)]
        public string? RecipientName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.CreditCard)]
        public string? RecipientAccount { get; set; }

        [Required]
        [StringLength(200)]
        public string? RecipientAddress { get; set; }

        [DataType(DataType.MultilineText)]
        public string? AddittionalInfo { get; set; }

        [Required]
        public int BankId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }
    }
}
