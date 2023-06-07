using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace TestMedicationInventoryManagement.Home;

[TestClass]
public class GetAllMedicationsTest
{
    private MMContext _context;
    private MedicationService _service;
    private Mock<IMapper>? _mapper;

    [TestInitialize]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<MMContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new MMContext(options);
        _mapper = new Mock<IMapper>();

        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        _context = new MMContext(
            new DbContextOptionsBuilder<MMContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .UseInternalServiceProvider(serviceProvider)
                .Options);

        _service = new MedicationService(_context, _mapper.Object);
    }

    [TestMethod]
    public async Task GetAllMedications_ReturnMedications_Exist()
    {
        // Arrange
        var medication1 = new Medication { MedicationId = Guid.NewGuid(),MedicationName = "Med1", ExpirationDate = DateTime.Now, Quantity = 10 };
        var medication2 = new Medication { MedicationId = Guid.NewGuid(), MedicationName = "Med2", ExpirationDate = DateTime.Now, Quantity = 20 };
        _context.Medications.Add(medication1);
        _context.Medications.Add(medication2);
        await _context.SaveChangesAsync();

        var medicationDto1 = new MedicationDTO { ExpirationDate = DateTime.Now, Quantity = 10 };
        var medicationDto2 = new MedicationDTO { ExpirationDate = DateTime.Now, Quantity = 20 };

        _mapper.Setup(m => m.Map<MedicationDTO>(medication1)).Returns(medicationDto1);
        _mapper.Setup(m => m.Map<MedicationDTO>(medication2)).Returns(medicationDto2);

        // Act
        var response = await _service.GetAllMedications();

        // Assert
        Assert.AreEqual(2, response.Count());
        Assert.AreEqual(medicationDto1.Quantity, response.ElementAt(0).Quantity);
        Assert.AreEqual(medicationDto2.Quantity, response.ElementAt(1).Quantity);
    }

}