namespace MedicationInventoryManagement.Models;

public class NotificationDTO
{
    public Guid NotificationId { get; set; }

    public string NotificationType { get; set; }

    public string NotificationMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    //public string Status { get; set; }

    //public MedicationDTO Medication { get; set; }

}