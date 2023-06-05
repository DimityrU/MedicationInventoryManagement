using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Services;

public class MedicationService : IMedicationService
{
    private readonly MMContext _context;
    private readonly IMapper _mapper;

    public MedicationService(MMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MedicationDTO>> GetAllMedications()
    {
        var medications = await _context.Medications.ToListAsync();
        var medicationResponse = new List<MedicationDTO>();

        foreach (var medication in medications)
        {
            medicationResponse.Add(_mapper.Map<MedicationDTO>(medication));
        }

        return medicationResponse;
    }

    public async Task<BaseResponse> AddMedication(MedicationDTO medicationRequest)
    {
        var response = new BaseResponse();

        try
        {
            if (medicationRequest.Quantity <= 0)
            {
                response.AddError("Medication quantity shouldn't be less than 0.");
            }

            if (medicationRequest.ExpirationDate < DateTime.Now.AddMonths(1))
            {
                response.AddError("Medication is expired or date is too soon to be added to the system.");
            }

            if (!response.Success)
            {
                return response;
            }
            var medication = _mapper.Map<Medication>(medicationRequest);
            await _context.Medications.AddAsync(medication);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            response.AddError("Error occurred while adding medication.");
        }

        return response;
    }

    public async Task RemoveMedication(Guid medicationId)
    {
        var medication =  await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == medicationId);
        if (medication == null) return;
        _context.Medications.Remove(medication);
         await _context.SaveChangesAsync();
    }

    public async Task ReduceQuantity(Guid medicationId, int newQuantity)
    {
        var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == medicationId);
        if (medication == null) return;
        var oldQuantity = medication.Quantity;
        if(oldQuantity < newQuantity || newQuantity <= 0) return;
        medication.Quantity = newQuantity;
        await _context.SaveChangesAsync();
    }
}