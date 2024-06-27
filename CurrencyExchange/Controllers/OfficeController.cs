using CurrencyExchange.Data;
using CurrencyExchange.Models;
using CurrencyExchange.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Controllers
{
    [Authorize]
    public class OfficeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public OfficeController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var applications = await _context.Applications
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a=>a.CreatedAt)
                .ToListAsync();

            var transfers = await _context.TransferRequests 
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var model = new OfficeVM()
            {
                Applications = applications,
                User = user,
                TransferRequests = transfers 
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync (User);
            if(user == null)
            {
                return NotFound();
            }
            var model = new EditUserVM
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return NotFound();
                }
                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;
                user.Address = model.Address;
                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return View(model);
        }
    }
}
