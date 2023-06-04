﻿using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;

namespace MedicationInventoryManagement.Services;

public class MedicationService : IMedicationService
{
    private readonly MMContext _context;

    public MedicationService(MMContext context)
    {
        _context = context;
    }

    public IEnumerable<Medication>? GetAllMedications()
    {
        return _context.Medications;
    }

    public void AddMedication(Medication medication)
    {
        medication.MedicationId = Guid.NewGuid();

        _context.Medications.Add(medication);
        _context.SaveChanges();
    }

    public void RemoveMedication(Guid medicationId)
    {
        var medication = _context.Medications.FirstOrDefault(m => m.MedicationId == medicationId);
        if (medication == null) return;
        _context.Medications.Remove(medication);
        _context.SaveChanges();
    }
}