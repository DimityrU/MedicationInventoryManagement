﻿using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Models.ViewModels;
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
        private readonly IOrderService _orderService;


        public HomeController(IMedicationService medicationService, INotificationsService notificationsService, IOrderService orderService)
        {
            _medicationService = medicationService;
            _notificationsService = notificationsService;
            _orderService = orderService;
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
            catch (Exception)
            {
                ModelState.AddModelError("", "No Medication in the inventory");
            }

            return View(new AllMedicationsViewModel());
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                {
                    var ordered = _orderService.CheckOrder(id);
                    if (!ordered)
                    {
                        await _notificationsService.DeleteNotification(id);
                        var response = await _medicationService.RemoveMedication(id);
                        if (!response.Success)
                        {
                            TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "There is an order for this medication, you cannot remove it!";
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
        public async Task<IActionResult> Reduce(Guid id, int newQuantity)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                {
                    var response = await _medicationService.ReduceQuantity(id, newQuantity);
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
