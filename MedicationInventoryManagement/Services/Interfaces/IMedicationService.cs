﻿using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface IMedicationService
{
    public Task<IEnumerable<MedicationDTO>> GetAllMedications();

    public Task<IEnumerable<MedicationDTO>> GetAllMedicationsForOrder();

    public Task<BaseResponse> AddMedication(MedicationDTO medicationRequest);

    public Task<BaseResponse> RemoveMedication(Guid medicationId);

    public Task<BaseResponse> ReduceQuantity(Guid medicationId, int newQuantity);
}