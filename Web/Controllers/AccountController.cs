using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.ViewModels.Account;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Email == model.Email);
                if (user is null)
                {
                    user = new User
                    {
                        Email = model.Email,
                        Password = model.Password
                    };

                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Email", "Email is already registered");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users
                    .FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError("", "Incorrect email or password");
            }

            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name,user.Email),
                new ("Id", user.Id.ToString()),
                new (ClaimTypes.Role, user.IsAdmin ? "Admin": "Bitard")
            };

            var identity = new ClaimsIdentity(claims, "Cheburek");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}