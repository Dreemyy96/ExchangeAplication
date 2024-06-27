namespace CurrencyExchange.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Quest { get; set; }
        public string? Answer { get; set; }
        public bool IsMain {  get; set; }
    }
}
