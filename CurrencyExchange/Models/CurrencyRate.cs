using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int Nominal { get; set; }
        public decimal SaleValue { get; set; }
        public decimal BuyValue { get; set; }
        public string? BankName { get; set; }
        public decimal? PreviousValue { get; set; }
    }
}
