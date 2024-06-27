using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using CurrencyExchange.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CurrencyExchange.Controllers
{
    
    public class ValuteController : Controller
    {
        private readonly AppDbContext _context;
        public ValuteController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            
            try
            {
                var currencies = await _context.CurrencyRates
                    .Where(a=>a.BankName=="ЦБ РФ")
                    .ToListAsync();

                CurrencyRateVM currencyRate = new()
                {
                    SourceValute = CurrencyEnum.AvailableValute.RUB,
                    TargetValute = CurrencyEnum.AvailableValute.USD,
                    Currencies = currencies!,
                    Amount = 1
                };

                return View(currencyRate);
            }catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(CurrencyRateVM model)
        {
            try
            {
                var currencies = await _context.CurrencyRates
                    .Where(a => a.BankName == "ЦБ РФ")
                    .ToListAsync();

                if (model.SourceValute == CurrencyEnum.AvailableValute.RUB)
                {
                    var targetValute = currencies
                    ?.Where(valute => valute.Code == model.TargetValute.ToString())
                    .Select(valute => new CurrencyRate
                    {
                        Code = valute.Code!,
                        Name = valute.Name!,
                        Nominal = valute.Nominal!,
                        SaleValue = valute.SaleValue,
                        BuyValue = valute.BuyValue
                    }).First();

                    if (targetValute == null) throw new NullReferenceException(nameof(targetValute));

                    decimal result = model.Amount/(targetValute.BuyValue / targetValute.Nominal);
                    model.Currencies = currencies!;
                    model.Result = Math.Round(result, 2);

                    return View(model);
                }
                else
                {
                    var sourceValute = currencies
                    ?.Where(valute => valute.Code == model.SourceValute.ToString())
                    .Select(valute => new CurrencyRate
                    {
                        Code = valute.Code!,
                        Name = valute.Name!,
                        Nominal = valute.Nominal!,
                        SaleValue = valute.SaleValue,
                        BuyValue= valute.BuyValue
                    }).First();

                    if (sourceValute == null) throw new NullReferenceException(nameof(sourceValute));

                    decimal result = (sourceValute.SaleValue/sourceValute.Nominal) * model.Amount;
                    model.Currencies = currencies!;
                    model.Result = Math.Round(result, 2);

                    return View(model);
                }      
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
