using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IMedicationService
{
    public Task<IEnumerable<MedicationDTO>> GetAllMedications();

    public Task<BaseResponse> AddMedication(MedicationDTO medicationRequest);

    public Task RemoveMedication(Guid medicationId);

    public Task ReduceQuantity(Guid medicationId, int newQuantity);
}