using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly INotificationsService _notificationsService;
        private readonly IMedicationService _medicationService;



        public OrdersController(IMedicationService medicationService, INotificationsService notificationsService)
        {
            _medicationService = medicationService;
            _notificationsService = notificationsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (TempData["ErrorMessage"] != null)
                {
                    ModelState.AddModelError("",
                        TempData["ErrorMessage"]?.ToString() ?? "Error occurred! Please try again!");
                }
                else
                {
                    var response = await _notificationsService.GetAllNotifications();


                    var allMedications = new AllMedicationsViewModel
                    {
                        Medications = null,
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var notificationResponse = await _notificationsService.GetAllNotifications();
                var medications = await _medicationService.GetAllMedications();

                var order = new OrderDTO
                {
                    OrderId = null,
                    OrderName = "",
                    OrderDate = null,
                    Status = "",
                    Medication = medications.Select(m => new OrderMedicationDTO { Medication = new MedicationDTO { MedicationId = m.MedicationId, MedicationName = m.MedicationName }, newQuantity = 0 }).ToList()
                };

                var response = new OrderViewModel
                {
                    Order = order,
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

        [HttpPost]
        public async Task<IActionResult> Create(OrderDTO order)
        {


            return RedirectToAction("Index");

        }
    }
}
