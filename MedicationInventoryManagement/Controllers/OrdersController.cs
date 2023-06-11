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
            var medications = await _medicationService.GetAllMedicationsForOrder();

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
        public async Task<IActionResult> Details(Guid id)
        {
            var model = new OrderViewModel();
            try
            {
                model = await CreateOrderViewModel();
                var order = await _orderService.GetOrder(id);
                model.Order = order;
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message ;
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Details(OrderDTO order)
        {
            try
            {
                if (order == null)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                {
                    var response = await _orderService.FinishOrder(order);
                    if (!response.Success)
                    {
                        TempData["ErrorMessage"] = "Cannot finish the order! Please try again";
                    }
                    else
                    {
                        foreach (var medication in order.OrderMedications)
                        {
                            await _notificationsService.DeleteNotification(medication.Medication.MedicationId,
                                "low quantity");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "System error, please try again later.";
                }
                else
                { 
                    var response = await _orderService.CancelOrder(id);
                    if (!response.Success)
                    {
                        TempData["ErrorMessage"] = response.Errors.FirstOrDefault().ErrorMessage;
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error occurred while canceling the order.";
            }

            return RedirectToAction("Index");
        }
    }
}
