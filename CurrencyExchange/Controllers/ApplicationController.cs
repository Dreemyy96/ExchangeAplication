using CurrencyExchange.Data;
using CurrencyExchange.Enums;
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using CurrencyExchange.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Globalization;
using System.Xml.Linq;

namespace CurrencyExchange.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext _context;

        public ApplicationController(UserManager<AppUser> userManager, AppDbContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> ExchangeApplication()
        {
            var user = await userManager.GetUserAsync(User);
            var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();

            ViewBag.Banks = new SelectList(banks, "Id", "Display");

            ApplicationVM dataUser = new()
            {
                FName = user?.Name,
                Email = user?.Email,
                Phone = user?.PhoneNumber,
                CurrencyFrom = CurrencyEnum.AvailableValute.RUB,
                CurrencyTo = CurrencyEnum.AvailableValute.USD,
                AppointmentDate = DateTime.Today
            };

            return View(dataUser);
        }



        [HttpPost]
        public async Task<IActionResult> SubmitApplication(ApplicationVM model)
        {
            if (!ModelState.IsValid)
            {
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("ExchangeApplication", model);
            }

            var appointmentDateTime = model.AppointmentDate.Date + model.AppointmentTime;

            if (appointmentDateTime < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Выбранная дата и время не могут быть в прошлом.");
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("ExchangeApplication", model);
            }

            var startTime = appointmentDateTime.AddMinutes(-15);
            var endTime = appointmentDateTime.AddMinutes(15);
            var existingAppointments = await _context.Applications
                .Where(a => a.ApointmentDateTime >= startTime && a.ApointmentDateTime <= endTime && a.BankId == model.BankId)
                .ToListAsync();

            if (existingAppointments.Any())
            {
                ModelState.AddModelError(string.Empty, "Выбранное время уже занято. Пожалуйста, выберите другое время.");
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("ExchangeApplication", model);
            }

            var appointmentTime = model.AppointmentTime;
            if (appointmentTime < new TimeSpan(9, 0, 0) || appointmentTime > new TimeSpan(18, 0, 0))
            {
                ModelState.AddModelError(string.Empty, "Выберите время между 09:00 и 18:00.");
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("ExchangeApplication", model);
            }

            var currencies = await _context.CurrencyRates.Where(a => a.BankName == "Альфа-Банк").ToListAsync();

            decimal result;

            if (model.CurrencyFrom == CurrencyEnum.AvailableValute.RUB)
            {
                var targetValute = currencies
                ?.Where(valute => valute.Code == model.CurrencyTo.ToString())
                .Select(valute => new CurrencyRate
                {
                    Code = valute.Code!,
                    Name = valute.Name!,
                    Nominal = valute.Nominal!,
                    SaleValue = valute.SaleValue,
                    BuyValue = valute.BuyValue
                }).First();

                if (targetValute == null) throw new NullReferenceException(nameof(targetValute));

                result = Math.Round(model.Amount / (targetValute.BuyValue / targetValute.Nominal), 2);
            }
            else
            {
                var sourceValute = currencies
                ?.Where(valute => valute.Code == model.CurrencyFrom.ToString())
                .Select(valute => new CurrencyRate
                {
                    Code = valute.Code!,
                    Name = valute.Name!,
                    Nominal = valute.Nominal!,
                    SaleValue = valute.SaleValue,
                    BuyValue = valute.BuyValue
                }).First();

                if (sourceValute == null) throw new NullReferenceException(nameof(sourceValute));

                result = Math.Round((sourceValute.SaleValue / sourceValute.Nominal) * model.Amount, 2);
            }

            var user = await userManager.GetUserAsync(User);

            var bank = await _context.Banks.FindAsync(model.BankId);

            Application appMod = new Application()
            {
                UserId = user?.Id,
                Amount = model.Amount,
                AmountDue = result,
                CurrencyFrom = model.CurrencyFrom.ToString(),
                CurrencyTo = model.CurrencyTo.ToString(),
                CreatedAt = DateTime.Now,
                AdditionalInfo = model.AdditionalInfo,
                Status = ApplicationStatus.Pending.ToString(),
                User = user,
                BankId = model.BankId,
                Bank = bank,
                ApointmentDateTime = appointmentDateTime
            };

            TempData["Application"] = JsonConvert.SerializeObject(appMod);

            return RedirectToAction("ConfirmApplication");
        }

        public IActionResult ConfirmApplication()
        {
            if (TempData["Application"] is string appJson)
            {
                var model = JsonConvert.DeserializeObject<Application>(appJson);
                return View(model);
            }

            return RedirectToAction("ExchangeApplication");
        }

        [HttpPost]
        public IActionResult SaveApplication(Application model)
        {
            if (ModelState.IsValid)
            {
                _context.Applications.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index", "Valute");
            }

            ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении заявки.");
            return View("ConfirmApplication", model);
        }
        public IActionResult RedirectWithAnchor(string anchor)
        {
            return Redirect($"/Application/ExchangeApplication#{anchor}");
        }

        [HttpGet]
        public async Task<IActionResult> GetConversionRate(string currencyFrom, string currencyTo, decimal amount)
        {
            var currencies = await _context.CurrencyRates.Where(a => a.BankName == "Альфа-Банк").ToListAsync();
            decimal result = 0;

            if (currencyFrom == "RUB")
            {
                var targetValute = currencies?.FirstOrDefault(valute => valute.Code == currencyTo && valute.BankName == "Альфа-Банк");
                if (targetValute != null)
                {
                    result = Math.Round(amount / (targetValute.BuyValue / targetValute.Nominal), 2);
                }
            }
            else
            {
                var sourceValute = currencies?.FirstOrDefault(valute => valute.Code == currencyFrom && valute.BankName== "Альфа-Банк");
                if (sourceValute != null)
                {
                    result = Math.Round((sourceValute.SaleValue / sourceValute.Nominal) * amount, 2);
                }
            }

            return Json(new { result });
        }

    }
}
