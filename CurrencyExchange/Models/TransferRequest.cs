using CurrencyExchange.CurrencyEnum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Models
{
    public class TransferRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }

        [ForeignKey("Bank")]
        public int? BankId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientAccount { get; set; }
        public string? RecipientAddress { get; set; }
        public string? AddittionalInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime ApointmentDateTime { get; set; }

        public AppUser? User { get; set; }
        public Bank? Bank { get; set; }
    }
    public enum TransferStatus
    {
        Pending,
        Completed,
        Canceled
    }
}
