namespace MedicationInventoryManagement.Models;

public class MedicationViewModel
{
    public MedicationDTO? Medication { get; set; }

    public IEnumerable<NotificationDTO> Notifications { get; set; }
}