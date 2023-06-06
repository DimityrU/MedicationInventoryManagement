namespace MedicationInventoryManagement.Models;

public class ModalData
{
    public Guid? Id { get; set; }
    public string MessageText { get; set; }
    public string ButtonText { get; set; }
    public string ButtonType { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }

    public int? Quantity { get; set; }
}