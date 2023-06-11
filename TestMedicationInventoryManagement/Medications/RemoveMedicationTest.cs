using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Home;

[TestClass]
public class RemoveMedicationTest
{
    private Mock<MMContext>? _mockContext;
    private Mock<DbSet<Medication>>? _mockSet;
    private MMContext _context;
    private IMedicationService _service;
    private Mock<IMapper>? _mapper;

    [TestInitialize]
    public void SetUp()
    {
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
    public async Task RemoveMedication_RemoveMedication_Exists()
    {
        // Arrange
        var medicationId = Guid.NewGuid();
        var medication = new Medication { MedicationId = medicationId, ExpirationDate = DateTime.Now, Quantity = 10, MedicationName = "Medication1" };
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.RemoveMedication(medicationId);

        // Assert
        Assert.IsTrue(response.Success);
        Assert.IsNull(await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == medicationId));
    }

    [TestMethod]
    public async Task RemoveMedication_ReturnError_MedicationDoesNotExist()
    {
        // Arrange
        var nonExistentMedicationId = Guid.NewGuid();

        // Act
        var response = await _service.RemoveMedication(nonExistentMedicationId);

        // Assert
        Assert.IsFalse(response.Success);
        Assert.AreEqual("Error occurred while deleting the medication!", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task RemoveMedication_ReturnError_ExceptionOccurs()
    {
        // Arrange

        _mockSet = new Mock<DbSet<Medication>>();
        _mockContext = new Mock<MMContext>();
        _mockContext.Setup(m => m.Medications).Returns(_mockSet.Object);
        _service = new MedicationService(_mockContext.Object, _mapper.Object);

        var medicationId = Guid.Empty;

        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var response = await _service.RemoveMedication(medicationId);

        // Assert
        Assert.IsFalse(response.Success);
        Assert.AreEqual("Error occurred while removing medication.", response.Errors.First().ErrorMessage);
    }

}