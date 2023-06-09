using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Models.ViewModels;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicationInventoryManagement.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly INotificationsService _notificationsService;
        private readonly IMedicationService _medicationService;

        public OrdersController(IOrderService orderService, IMedicationService medicationService,
            INotificationsService notificationsService)
        {
            _orderService = orderService;
            _medicationService = medicationService;
            _notificationsService = notificationsService;
        }
        private async Task<OrderViewModel> CreateOrderViewModel()
        {
            var notificationResponse = await _notificationsService.GetAllNotifications();
            var medications = await _medicationService.GetAllMedications();

            var order = new OrderDTO
            {
                OrderId = null,
                OrderName = "",
                OrderDate = null,
                Status = "",
                OrderMedications = medications.Select(m => new OrderMedicationDTO
                {
                    Medication = new MedicationDTO
                    {
                        MedicationId = m.MedicationId,
                        MedicationName = m.MedicationName
                    },
                    NewQuantity = 0
                }).ToList()
            };

            var response = new OrderViewModel
            {
                Order = order,
                Notifications = notificationResponse.Notifications
            };

            return response;
        }

        [Authorize]
        [HttpGet]
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
                    var response = await _orderService.GetAllShippedOrders();
                    var notificationResponse = await _notificationsService.GetAllNotifications();

                    if (!response.Success)
                    {
                        TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
                    }

                    var model = new AllOrdersViewModel
                    {
                        Orders = response.Orders,
                        Notifications = notificationResponse.Notifications
                    };
                    return View(model);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "No Medication in the inventory");
            }

            return View(new AllOrdersViewModel());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = await CreateOrderViewModel();
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while redirecting.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(OrderDTO order)
        {
            var response = await _orderService.PlaceOrder(order);

            if (response.Success) return RedirectToAction("Index");

            TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
            var model = await CreateOrderViewModel();
            return View(model);

        }

        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            return View();
        }

        public IActionResult Cancel(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
