using CurrencyExchange.CurrencyEnum;
using CurrencyExchange.Models;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.ViewModels
{
    public class CurrencyRateVM
    {
        public List<CurrencyRate> Currencies { get; set; } = new List<CurrencyRate>();

        [Required]
        public AvailableValute SourceValute { get; set; }

        [Required]
        public AvailableValute TargetValute { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency)]
        public decimal Result {  get; set; }


    }
}
