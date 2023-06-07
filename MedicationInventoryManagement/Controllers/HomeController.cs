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
        private readonly INotificationsService _notificationsService;


        public HomeController(IMedicationService medicationService, INotificationsService notificationsService)
        {
            _medicationService = medicationService;
            _notificationsService = notificationsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (TempData["ErrorMessage"] != null)
                {
                    ModelState.AddModelError("", TempData["ErrorMessage"]?.ToString() ?? "Error occurred! Please try again!");
                }
                else
                {
                    var response = await _notificationsService.GetAllNotifications();
                    var medications = await _medicationService.GetAllMedications();

                    foreach (var medication in medications)
                    {
                        await _notificationsService.CheckLowQuantityNotification(medication.MedicationId);
                        await _notificationsService.CheckExpirationDateNotification(medication.MedicationId);
                    }

                    var allMedications = new AllMedicationsViewModel
                    {
                        Medications = medications,
                        Notifications = response.Notifications
                    };

                    return View(allMedications);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "No Medication in the inventory");
            }

            return View(new AllMedicationsViewModel());
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
                    await _notificationsService.DeleteNotification(medicationId);
                    var response = await _medicationService.RemoveMedication(medicationId);
                    if (!response.Success)
                    {
                        TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
                    }
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
        public async Task<IActionResult> Create()
        {
            try
            {
                var notificationResponse = await _notificationsService.GetAllNotifications();

                var response = new MedicationViewModel
                {
                    Medication = null,
                    Notifications = notificationResponse.Notifications
                };
                return View(response);
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
            var notificationResponse = await _notificationsService.GetAllNotifications();
            var medicationResponse = await _medicationService.AddMedication(medication);

            var response = new MedicationViewModel
            {
                Medication = medication,
                Notifications = notificationResponse.Notifications
            };

            if (notificationResponse.Success && medicationResponse.Success) return RedirectToAction("Index");
            foreach (var error in medicationResponse.Errors)
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }
            foreach (var error in notificationResponse.Errors)
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }

            return View(response);

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
                    var response = await _medicationService.ReduceQuantity(medicationId, newQuantity);
                    if (!response.Success)
                    {
                        TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
                    }
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
