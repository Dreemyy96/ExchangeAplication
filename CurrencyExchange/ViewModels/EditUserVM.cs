using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.ViewModels
{
    public class EditUserVM
    {
        [StringLength(100)]
        public string? Name { get; set; }


        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [StringLength(12)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }
    }
}
