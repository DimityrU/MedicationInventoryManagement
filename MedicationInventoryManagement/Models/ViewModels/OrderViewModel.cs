namespace MedicationInventoryManagement.Models.ViewModels;

public class OrderViewModel
{
    public OrderDTO? Order { get; set; }

    public IEnumerable<NotificationDTO> Notifications { get; set; }

}