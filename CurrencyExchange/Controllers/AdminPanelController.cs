using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurrencyExchange.Data;
using CurrencyExchange.Enums;
using CurrencyExchange.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using CurrencyExchange.Models;
using System.Runtime.CompilerServices;
using CurrencyExchange.Services;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.Xml;

namespace CurrencyExchange.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public AdminPanelController(AppDbContext context, IEmailService service)
        {
            _context = context;
            _emailService = service;
        }

        public async Task<IActionResult> Index(string? bankAddress = null)
        {
            var applicationsQuery = _context.Applications
                .Include(a => a.User)
                .Include(a => a.Bank)
                .Where(a => a.Status == ApplicationStatus.Pending.ToString());

            var transfersQuery = _context.TransferRequests
                .Include(a => a.User)
                .Include(a => a.Bank)
                .Where(a => a.Status == TransferStatus.Pending.ToString());

            if (!string.IsNullOrEmpty(bankAddress))
            {
                applicationsQuery = applicationsQuery.Where(a => a.Bank!.Address == bankAddress);
                transfersQuery = transfersQuery.Where(a => a.Bank!.Address == bankAddress);
            }

            var applications = await applicationsQuery.ToListAsync();
            var transfers = await transfersQuery.ToListAsync();
            var questions = await _context.Questions.ToListAsync();
            var banks = await _context.Banks.ToListAsync();

            var model = new AdminPanelVM()
            {
                Applications = applications,
                Transfers = transfers,
                Questions = questions,
                Banks = banks
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateApplicationStatus(int ApplicationId, string ApplicationStatus, string? RejectionReason)
        {
            var application = await _context.Applications
                                    .Include(a => a.User)
                                    .Include(a => a.Bank)
                                    .FirstOrDefaultAsync(a => a.AplicationId == ApplicationId);
            if (application != null)
            {
                application.Status = ApplicationStatus;
                if (ApplicationStatus == Enums.ApplicationStatus.Rejected.ToString() && RejectionReason != null)
                {
                    application.RejectionReason = RejectionReason;
                }
                _context.Update(application);
                await _context.SaveChangesAsync();
            }

            var userEmail = _context.Users
                .Where(u => u.Id == application!.UserId)
                .First().Email;

            if (ApplicationStatus == Enums.ApplicationStatus.Approved.ToString())
            {
                var subject = "Статус вашей заявки на обмен валюты был изменен!";
                var body = $"Здравствуйте, {application?.User?.Name}!" +
                    $"\n\nВаша заявка была утверждена" +
                    $"\nДля операции обмена валюты пройдите по адресу в назначенное время." +
                    $"\nАдрес: {application?.Bank?.Address}" +
                    $"\nВремя и дата:{application?.ApointmentDateTime}" +
                    $"\nПри себе иметь паспорт и {application?.Amount} {application?.CurrencyFrom}" +
                    $"\n\nС наилучшими пожеланиями,\nCurrencyExchange Team";
                if (userEmail != null)
                    await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            else if (ApplicationStatus == Enums.ApplicationStatus.Rejected.ToString())
            {
                var subject = "Статус вашей заявки на обмен валюты был изменен!";
                var body = $"Здравствуйте, {application?.User?.Name}!" +
                    $"\n\nВаша заявка была отклонена" +
                    $"\nПричина отклонения заявки: {application?.RejectionReason} " +
                    $"\n\nС наилучшими пожеланиями,\nCurrencyExchange Team";
                if (userEmail != null)
                    await _emailService.SendEmailAsync(userEmail, subject, body);
            }


            var notification = new Notification()
            {
                UserId = application?.UserId,
                NotificationType = "Статус обмена валюты",
                Msg = $"Ваша заявка от {application?.CreatedAt.ToString()} " +
                $"была {(ApplicationStatus == Enums.ApplicationStatus.Approved.ToString() ? "утверждена." : "отклонена.")}" +
                $"\nБолее подробную информацию отправили на почту!"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> UpdateTransferStatus(int TransferId, string TransferStatus, string? RejectionReason)
        {
            var transfer = await _context.TransferRequests
                                        .Include(a => a.User)
                                        .Include(a => a.Bank)
                                        .FirstOrDefaultAsync(a => a.Id == TransferId);
            if (transfer != null)
            {
                transfer.Status = TransferStatus;
                if (TransferStatus == Models.TransferStatus.Canceled.ToString() && RejectionReason != null)
                {
                    transfer.RejectionReason = RejectionReason;
                }
                _context.Update(transfer);
                await _context.SaveChangesAsync();
            }

            var userEmail = _context.Users
                .Where(u => u.Id == transfer!.UserId)
                .First().Email;
            if (TransferStatus == Models.TransferStatus.Completed.ToString())
            {
                var subject = "Статус вашей заявки на международный перевод был изменен!";
                var body = $"Здравствуйте, {transfer?.User?.Name}!" +
                    $"\n\nСтатус вашей заявки был изменен на {TransferStatus}" +
                    $"\nДля совершения перевода пройдите по адресу в назначенное время." +
                    $"\nАдрес: {transfer?.Bank?.Address}" +
                    $"\nВремя:{transfer?.ApointmentDateTime}" +
                    $"\nПри себе иметь паспорт и {transfer?.Amount} {transfer?.Currency}" +
                    $"\n\nС наилучшими пожеланиями,\nCurrencyExchange Team";
                if (userEmail != null)
                    await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            else if (TransferStatus == Models.TransferStatus.Canceled.ToString())
            {
                var subject = "Статус вашей заявки на международный перевод был изменен!";
                var body = $"Здравствуйте, {transfer?.User?.Name}!" +
                    $"\n\nВаша заявка была отклонена" +
                    $"\nПричина отклонения заявки: {transfer?.RejectionReason} " +
                    $"\n\nС наилучшими пожеланиями,\nCurrencyExchange Team";
                if (userEmail != null)
                    await _emailService.SendEmailAsync(userEmail, subject, body);
            }

            var notification = new Notification()
            {
                UserId = transfer?.UserId,
                NotificationType = "Статус перевода",
                Msg = $"Статус вашей заявки от {transfer?.CreatedAt.ToShortDateString()}" +
                $" был изменен на {(TransferStatus == Models.TransferStatus.Completed.ToString() ? "утверждена." : "отклонена.")}" +
                $"\nБолее подробную информацию отправили на почту!"
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> UpdateQuestionIsMain(int questionId, bool isMain)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                question.IsMain = isMain;
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer(int questionId, string answer)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                question.Answer = answer;
                _context.Update(question);
                await _context.SaveChangesAsync();
            }

            if(answer != null)
            {
                var userEmail = question?.Email;
                var subject = "На Ваш Вопрос Был Получен Ответ!";
                var body = $"Здравствуйте, {question?.Name}!\n\nНа ваш вопрос:\n{question?.Quest}\nБыл получен ответ:\n{answer}" +
                    $"\n\nС наилучшими пожеланиями,\nCurrencyExchange Team";

                if (userEmail != null)
                    await _emailService.SendEmailAsync(userEmail, subject, body);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
