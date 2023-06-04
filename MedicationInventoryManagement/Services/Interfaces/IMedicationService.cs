using MedicationInventoryManagement.Entities;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IMedicationService
{
    public Task<IEnumerable<Medication>> GetAllMedications();

    public Task AddMedication(Medication medication);

    public Task RemoveMedication(Guid medicationId);

    public Task ReduceQuantity(Guid medicationId, int newQuantity);
}