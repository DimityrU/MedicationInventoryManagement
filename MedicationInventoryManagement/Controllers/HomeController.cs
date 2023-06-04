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
            if (TempData["ErrorMessage"] != null)
            {
                ModelState.AddModelError("", TempData["ErrorMessage"]?.ToString() ?? "Error occur! Please try again!");
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
                if (medicationId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                    return RedirectToAction("Index");
                }
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

            if (medication.Quantity <= 0)
            {
                ModelState.AddModelError("", "Medication quantity shouldn't be less than 0.");
                return View();
            }
            else if(medication.ExpirationDate < DateTime.Now.AddMonths(1))
            {
                ModelState.AddModelError("", "Medication is expired or date is too soon to be added to the system.");
                return View();
            }

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
            try
            {
                if (medicationId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                    return RedirectToAction("Index");
                }

                _medicationService.ReduceQuantity(medicationId, newQuantity);
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while reducing the medication.";
                return RedirectToAction("Index");
            }

        }
    }
}
