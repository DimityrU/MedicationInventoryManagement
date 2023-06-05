using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IMedicationService
{
    public Task<IEnumerable<Medication>> GetAllMedications();

    public Task<BaseResponse> AddMedication(Medication medication);

    public Task RemoveMedication(Guid medicationId);

    public Task ReduceQuantity(Guid medicationId, int newQuantity);
}