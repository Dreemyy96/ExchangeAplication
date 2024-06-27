using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public NotificationController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _context = appDbContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var nots = await _context.Notifications
                .Where(u => u.UserId == user!.Id && u.CreatedAt>DateTime.Now.AddDays(-2))
                .OrderByDescending(n=>n.CreatedAt)
                .ToListAsync();
            return PartialView(nots);
        }
    }
}
