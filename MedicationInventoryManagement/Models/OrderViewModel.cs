namespace MedicationInventoryManagement.Models;

public class OrderViewModel
{
    public OrderDTO Order { get; set; }

    public IEnumerable<NotificationDTO> Notifications { get; set; }

}