namespace MedicationInventoryManagement.Models.ViewModels;

public class AllOrdersViewModel
{
    public IEnumerable<OrderDTO>? Orders { get; set; }
    public IEnumerable<NotificationDTO> Notifications { get; set; }
}