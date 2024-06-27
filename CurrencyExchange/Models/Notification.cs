using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        [Required]
        [StringLength(50)]
        public string? NotificationType{ get; set; }

        [Required]
        [StringLength(150)]
        public string? Msg { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public enum NotificationType
    {
        CurrencyExchange,
        Transfer,
        Question
    }
}
