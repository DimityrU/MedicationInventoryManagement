using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement
{

    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Medication, MedicationDTO>().ReverseMap();
        }
    }

}
