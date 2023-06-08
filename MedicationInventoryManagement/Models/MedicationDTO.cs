using MedicationInventoryManagement.Entities;

namespace MedicationInventoryManagement.Models;

public class MedicationDTO
{
    public Guid? MedicationId { get; set; }

    public string MedicationName { get; set; }
    public int? Quantity { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public List<NotificationDTO>? Notifications { get; set; }

    public List<OrderDTO>? Orders { get; set; }
}