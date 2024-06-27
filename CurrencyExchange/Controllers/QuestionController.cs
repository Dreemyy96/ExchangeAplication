using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Controllers
{
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;
        public QuestionController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var questions = await _context.Questions
                .Where(a => a.IsMain)
                .ToListAsync();

            var model = new QuestionsVM()
            {
                MainQuestions = questions
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(QuestionsVM model)
        {
            if (ModelState.IsValid)
            {
                var question = new Question
                {
                    Name = model.Name,
                    Email = model.Email,
                    Quest = model.Quest,
                    IsMain = false 
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var questions = await _context.Questions
                .Where(a => a.IsMain)
                .ToListAsync();
            model.MainQuestions = questions;

            return View("Index", model);
        }
    }
}
