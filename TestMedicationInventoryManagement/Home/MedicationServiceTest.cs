using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestMedicationInventoryManagement.Home;

[TestClass]
public class MedicationServiceTest
{
    private Mock<MMContext>? _mockContext;
    private Mock<DbSet<Medication>>? _mockSet;
    private MedicationService _service;
    private Mock<IMapper>? _mapper;

    [TestInitialize]
    public void SetUp()
    {
        _mockSet = new Mock<DbSet<Medication>>();
        _mockContext = new Mock<MMContext>();
        _mockContext.Setup(m => m.Medications).Returns(_mockSet.Object);
        _mapper = new Mock<IMapper>();

        _service = new MedicationService(_mockContext.Object, _mapper.Object);
    }

    [TestMethod]
    public async Task AddMedication_ValidMedication_AddsMedicationToDatabase()
    {
        var medication = new MedicationDTO()
        {
            MedicationName = "Test Medication",
            Quantity = 5,
            ExpirationDate = DateTime.Now.AddMonths(6)
        };

        await _service.AddMedication(medication);

        _mockSet?.Verify(m => m.AddAsync(It.IsAny<Medication>(), default), Times.Once());
        _mockContext?.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
}