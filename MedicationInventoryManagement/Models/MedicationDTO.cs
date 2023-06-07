using MedicationInventoryManagement.Entities;

namespace MedicationInventoryManagement.Models;

public class MedicationDTO
{
    public Guid? MedicationId { get; set; }

    public string MedicationName { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }

    public List<Notification>? Notification { get; set; }

    public List<Order>? Order { get; set; }
}