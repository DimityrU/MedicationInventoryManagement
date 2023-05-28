using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicationInventoryManagement.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogInService _logInService;

        public LogInController(ILogInService logInService)
        {
            _logInService = logInService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(User user)
        {
            bool isValidUser = _logInService.ValidateUser(user.UserName, user.Password);

            if (isValidUser)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    // Add additional claims if needed
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user
                await HttpContext.SignInAsync(principal, new AuthenticationProperties
                {
                    IsPersistent = true, // Set to true if you want to persist the authentication across sessions
                    ExpiresUtc = DateTime.UtcNow.AddHours(2), // Set the expiration time for the authentication cookie
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View("Index", user);
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "LogIn");
        }
    }
}
