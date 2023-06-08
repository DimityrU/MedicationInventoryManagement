using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Contracts;

public class AllOrdersResponse : BaseResponse
{
    public List<OrderDTO>? Orders { get; set; }

}