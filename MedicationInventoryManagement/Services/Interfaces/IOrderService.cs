using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IOrderService
{
    public Task<BaseResponse> PlaceOrder(OrderDTO orderDTO);

    public Task<AllOrdersResponse> GetAllShippedOrders();

    public bool CheckOrder(Guid id);

    public Task<BaseResponse> CancelOrder(Guid id);
}