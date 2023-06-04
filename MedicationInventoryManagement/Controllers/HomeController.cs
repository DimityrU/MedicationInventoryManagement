using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IMedicationService _medicationService;

        public HomeController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var allMedications = _medicationService.GetAllMedications()?.ToList();

            if (allMedications != null) return View(allMedications);

            ModelState.AddModelError("", "No Medication in the inventory");
            return View();

        }

        [Authorize]
        public IActionResult Delete(Guid medicationId)
        {
            //TODO: Check if medication have valid data

            _medicationService.RemoveMedication(medicationId);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Medication medication)
        {
            _medicationService.AddMedication(medication);

            return RedirectToAction("Index");

        }
    }
}
