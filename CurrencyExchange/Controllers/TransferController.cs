using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CurrencyExchange.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public TransferController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
            ViewBag.Banks = new SelectList(banks, "Id", "Display");
            var model = new TransferRequestVM()
            {
                Name = user?.Name,
                Address = user?.Address,
                Email = user?.Email,
                AppointmentDate = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(TransferRequestVM model)
        {
            if (!ModelState.IsValid)
            {
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("Index", model);
            }

            var appointmentDateTime = model.AppointmentDate.Date + model.AppointmentTime;

            if (appointmentDateTime < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Выбранная дата и время не могут быть в прошлом.");
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("Index", model);
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
                return View("Index", model);
            }

            var appointmentTime = model.AppointmentTime;
            if (appointmentTime < new TimeSpan(9, 0, 0) || appointmentTime > new TimeSpan(18, 0, 0))
            {
                ModelState.AddModelError(string.Empty, "Выберите время между 09:00 и 18:00.");
                var banks = await _context.Banks.Select(b => new { b.Id, Display = b.Name + " - " + b.Address }).ToListAsync();
                ViewBag.Banks = new SelectList(banks, "Id", "Display");
                return View("Index", model);
            }

            decimal res = model.Amount * 1.04m;

            var user = await _userManager.GetUserAsync(User);
            var bank = await _context.Banks.FindAsync(model.BankId);

            var transfer = new TransferRequest()
            {
                UserId = user?.Id,
                Amount = Math.Round(res, 2),
                Currency = model.Currency.ToString(),
                RecipientName = model.RecipientName,
                RecipientAddress = model.RecipientAddress,
                RecipientAccount = model.RecipientAccount,
                AddittionalInfo = model.AddittionalInfo,
                CreatedAt = DateTime.Now,
                Status = TransferStatus.Pending.ToString(),
                User = user,
                BankId = model.BankId,
                Bank = bank,
                ApointmentDateTime = appointmentDateTime
            };

            TempData["TransferRequest"] = JsonConvert.SerializeObject(transfer);
            return RedirectToAction("ConfirmTransfer");
        }

        [HttpGet]
        public IActionResult ConfirmTransfer()
        {
            if (TempData["TransferRequest"] is string appJson)
            {
                var model = JsonConvert.DeserializeObject<TransferRequest>(appJson);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ConfirmTransfer(TransferRequest model)
        {
            if (ModelState.IsValid)
            {
                _context.TransferRequests.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index", "Valute");
            }

            ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении заявки.");
            return View("ConfirmTransfer", model);
        }
    }
}
