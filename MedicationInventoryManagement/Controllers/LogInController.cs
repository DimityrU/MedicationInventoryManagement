using MedicationInventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    public class LogInController : Controller
    {
        public IActionResult Index(User user)
        {
            return View();
        }
    }
}
