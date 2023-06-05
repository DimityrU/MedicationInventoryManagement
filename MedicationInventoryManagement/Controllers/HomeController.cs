using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
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
        public async Task<IActionResult> Index()
        {
            try
            {
                if (TempData["ErrorMessage"] != null)
                {
                    ModelState.AddModelError("", TempData["ErrorMessage"]?.ToString() ?? "Error occur! Please try again!");
                }
                else
                {
                    var allMedications = await _medicationService.GetAllMedications();
                    return View(allMedications);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "No Medication in the inventory");
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid medicationId)
        {
            try
            {
                if (medicationId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                {
                    await _medicationService.RemoveMedication(medicationId);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while deleting the medication.";
            }

            return RedirectToAction("Index");
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
        public async Task<IActionResult> Create(MedicationDTO medication)
        {
            var response = await _medicationService.AddMedication(medication);

            if (response.Success) return RedirectToAction("Index");
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }

            return View(medication);

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Reduce(Guid medicationId, int newQuantity)
        {
            try
            {
                if (medicationId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                {
                    await _medicationService.ReduceQuantity(medicationId, newQuantity);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while reducing the medication.";
            }

            return RedirectToAction("Index");
        }
    }
}
