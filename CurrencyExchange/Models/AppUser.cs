using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Models
{
    public class AppUser : IdentityUser
    {
        [Key]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Required]
        [StringLength(100)]
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? Address { get; set; }

        public ICollection<Application>? Applications { get; set; }
    }
}
