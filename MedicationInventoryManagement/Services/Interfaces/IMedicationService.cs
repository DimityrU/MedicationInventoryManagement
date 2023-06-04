using MedicationInventoryManagement.Entities;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IMedicationService
{
    public IEnumerable<Medication>? GetAllMedications();

    public void AddMedication(Medication medication);

    public void RemoveMedication(Guid medicationId);
}