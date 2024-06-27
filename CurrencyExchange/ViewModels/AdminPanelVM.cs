using CurrencyExchange.Models;

namespace CurrencyExchange.ViewModels
{
    public class AdminPanelVM
    {
        public IEnumerable<Application>? Applications { get; set; }
        public IEnumerable<TransferRequest>? Transfers { get; set; }
        public IEnumerable<Question>? Questions { get; set; }
        public IEnumerable<Bank>? Banks { get; set; }
        public int ApplicationId { get; set; }
        public string? ApplicationStatus { get; set; }
        public int TransferId { get; set; }
        public string? TransferStatus { get; set; }
    }
}
