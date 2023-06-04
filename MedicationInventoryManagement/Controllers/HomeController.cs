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
            if (TempData["ErrorMessage"] == null)
            {
                ModelState.AddModelError("", "Bombaaaaaaaaaaaaaa");
                return View();
            }

            try
            {
                var allMedications = _medicationService.GetAllMedications()?.ToList();
                return View(allMedications);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "No Medication in the inventory");
                return View();
            }
        }

        [Authorize]
        public IActionResult Delete(Guid medicationId)
        {
            try
            {
                _medicationService.RemoveMedication(medicationId);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while deleting the medication.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while redirecting.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Medication medication)
        {

            try
            {
                _medicationService.AddMedication(medication);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while adding medication.";
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public IActionResult Reduce(Guid medicationId, int newQuantity)
        {
            _medicationService.ReduceQuantity(medicationId, newQuantity);

            return RedirectToAction("Index");
        }
    }
}
