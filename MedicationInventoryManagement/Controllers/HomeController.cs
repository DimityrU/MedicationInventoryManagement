using MedicationInventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
