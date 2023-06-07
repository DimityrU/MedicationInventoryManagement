using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Contracts;

public class NotificationResponse : BaseResponse
{
    public List<NotificationDTO> Notifications { get; set; }
}