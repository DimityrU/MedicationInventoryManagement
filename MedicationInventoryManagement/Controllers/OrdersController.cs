using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly INotificationsService _notificationsService;


        public OrdersController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Create()
        {
            try
            {
                var notificationResponse = await _notificationsService.GetAllNotifications();

                var response = new OrderViewModel
                {
                    Order = null,
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
    }
}
