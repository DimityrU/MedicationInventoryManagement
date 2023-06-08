namespace MedicationInventoryManagement.Models.ViewModels;

public class AllMedicationsViewModel
{
    public IEnumerable<MedicationDTO> Medications { get; set; }
    public IEnumerable<NotificationDTO> Notifications { get; set; }
}