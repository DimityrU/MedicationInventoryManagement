namespace MedicationInventoryManagement.Models;

public class OrderDTO
{   
    public Guid? OrderId { get; set; }

    public string? OrderName { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public List<OrderMedicationDTO>  OrderMedications { get; set; }
}