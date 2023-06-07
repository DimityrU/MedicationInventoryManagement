using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            var response = await _notificationsService.GetAllNotifications();

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
            }

            return PartialView("~/Views/Shared/_NotificationPanel.cshtml", response.Notifications.AsEnumerable());
        }

    }
}
