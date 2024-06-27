using CurrencyExchange.Models;

namespace CurrencyExchange.ViewModels
{
    public class OfficeVM
    {
        public AppUser? User { get; set; }
        public List<Application>? Applications { get; set; }
        public List<TransferRequest>? TransferRequests { get; set; }
    }
}
