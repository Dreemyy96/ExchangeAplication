using CurrencyExchange.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Models
{
    public class Application
    {
        [Key]
        public int AplicationId { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [ForeignKey("Bank")]
        public int? BankId { get; set; }

        public decimal Amount { get; set; }
        public decimal AmountDue { get; set; }
        public string? CurrencyFrom { get; set; }
        public string? CurrencyTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ApointmentDateTime { get; set; }


        public AppUser? User { get; set; }
        public Bank? Bank { get; set; }
    }
}
