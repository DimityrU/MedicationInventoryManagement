namespace MedicationInventoryManagement.Models;

public class OrderMedicationDTO
{
    public Guid? OrderMedicationID { get; set; }

    public MedicationDTO Medication { get; set; }

    public OrderDTO Order { get; set; }

    public int newQuantity { get; set; }

}