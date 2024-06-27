using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Models
{
    public class Bank
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(200)]
        public string? Address { get; set; }
    }
}
