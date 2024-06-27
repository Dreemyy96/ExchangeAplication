using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;

namespace CurrencyExchange.Services
{
    public class ExchangeRateUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;
        public ExchangeRateUpdater(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            _serviceProvider = serviceProvider;
            _httpClient = httpClient;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateCurrencyRates();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        private async Task UpdateCurrencyRates()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("https://www.cbr-xml-daily.ru/daily_json.js");

            var json = JObject.Parse(response);
            var rates = new List<CurrencyRate>();

            var numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = ""
            };

            foreach (var token in json["Valute"]!)
            {
                var code = token.First!["CharCode"]?.ToString();
                var name = token.First["Name"]?.ToString();
                var nominal = token.First["Nominal"]?.ToObject<int>() ?? 1;
                var valueString = token.First["Value"]?.ToString();
                var previousValueString = token.First["Previous"]?.ToString();

                if (decimal.TryParse(valueString, NumberStyles.Any, numberFormat, out var value) &&
                    decimal.TryParse(previousValueString, NumberStyles.Any, numberFormat, out var previousValue))
                {
                    rates.Add(new CurrencyRate
                    {
                        Code = code,
                        Name = name,
                        Nominal = nominal,
                        SaleValue = value,
                        BuyValue = value,
                        BankName = "ЦБ РФ",
                        PreviousValue = previousValue
                    });
                }
            }
            rates.AddRange(await GetRatesFromAlfaBank());

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var oldRates = await context.CurrencyRates.ToListAsync();
                context.CurrencyRates.RemoveRange(oldRates);

                context.CurrencyRates.AddRange(rates);
                await context.SaveChangesAsync();
            }
        }

        private async Task<List<CurrencyRate>> GetRatesFromAlfaBank()
        {
            var rates = new List<CurrencyRate>();
            var response = await _httpClient.GetStringAsync("https://developerhub.alfabank.by:8273/partner/1.0.0/public/rates");
            var json = JObject.Parse(response);

            var numberFormat = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = ""
            };

            foreach (var rate in json["rates"]!)
            {
                if (rate["buyIso"]?.ToString() == "RUB")
                {
                    var code = rate["sellIso"]?.ToString();
                    var saleValue = rate["sellRate"]!.ToString();
                    var nominal = int.Parse(rate["quantity"]!.ToString());
                    var buyValue = rate["buyRate"]!.ToString();

                    if (decimal.TryParse(saleValue, NumberStyles.Any, numberFormat, out var sValue) &&
                        decimal.TryParse(buyValue, NumberStyles.Any, numberFormat, out var bValue))
                    {
                        rates.Add(new CurrencyRate
                        {
                            Code = code,
                            Name = code,
                            Nominal = nominal,
                            SaleValue = sValue,
                            BuyValue = bValue,
                            BankName = "Альфа-Банк"
                        });
                    }
                }
                if (rate["buyIso"]?.ToString() == "BYN" && rate["sellIso"]?.ToString() == "RUB")
                {
                    var code = rate["buyIso"]?.ToString();
                    var saleValue = rate["sellRate"]!.ToString();
                    var nominal = int.Parse(rate["quantity"]!.ToString());
                    var buyValue = rate["buyRate"]!.ToString();

                    if (decimal.TryParse(saleValue, NumberStyles.Any, numberFormat, out var sValue) &&
                        decimal.TryParse(buyValue, NumberStyles.Any, numberFormat, out var bValue))
                    {
                        rates.Add(new CurrencyRate
                        {
                            Code = code,
                            Name = code,
                            Nominal = 1,
                            SaleValue = Math.Round((nominal/bValue)*10, 2),
                            BuyValue = Math.Round((nominal / sValue)*10, 2),
                            BankName = "Альфа-Банк"
                        });
                    }
                }
            }

            return rates;
        }

    }
}
