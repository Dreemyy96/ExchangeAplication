using System.Xml.Linq;

namespace CurrencyExchange.Services
{
    public class ValuteApiClient
    {
        private readonly HttpClient httpClient;
        public ValuteApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<XDocument> GetApiResponseAsync(string apiUrl)
        {
            HttpResponseMessage responseMessage = await httpClient.GetAsync(apiUrl);
            if(responseMessage.IsSuccessStatusCode)
            {
                string responseData = await responseMessage.Content.ReadAsStringAsync();
                return XDocument.Parse(responseData);
            }
            else
            {
                responseMessage.EnsureSuccessStatusCode();
                return null!;
            }
        }
    }
}
