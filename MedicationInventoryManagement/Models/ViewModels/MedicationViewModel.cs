namespace MedicationInventoryManagement.Models.ViewModels;

public class MedicationViewModel
{
    public MedicationDTO? Medication { get; set; }

    public IEnumerable<NotificationDTO> Notifications { get; set; }
}