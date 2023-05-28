using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult LogIn(User user)
        {
            bool isValidUser = _logInService.ValidateUser(user.UserName, user.Password);

            if (isValidUser)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View("Index",user);
            }
        }
    }
}
