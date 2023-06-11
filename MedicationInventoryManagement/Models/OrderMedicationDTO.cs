namespace MedicationInventoryManagement.Models;

public class OrderMedicationDTO
{
    public Guid? OrderMedicationId { get; set; }

    public MedicationDTO Medication { get; set; }

    public OrderDTO Order { get; set; }

    public int NewQuantity { get; set; }

}