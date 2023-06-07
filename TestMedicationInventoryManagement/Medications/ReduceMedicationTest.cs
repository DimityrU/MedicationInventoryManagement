using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Home;

[TestClass]
public class ReduceMedicationTest
{
    private Mock<MMContext>? _mockContext;
    private Mock<DbSet<Medication>>? _mockSet;
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
    public async Task ReduceQuantity_ReturnError_MedicationDoesNotExist()
    {
        // Arrange
        var medicationId = Guid.NewGuid();

        // Act
        var response = await _service.ReduceQuantity(medicationId, 5);

        // Assert
        Assert.IsFalse(response.Success);
        Assert.AreEqual("Error occurred while reducing the quantity of the medication!", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task ReduceQuantity_ReturnError_NewQuantityIsInvalid()
    {
        // Arrange
        var medicationId = Guid.NewGuid();
        var medication = new Medication { MedicationId = medicationId, MedicationName = "Med1", Quantity = 10 };

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.ReduceQuantity(medicationId, 20);

        // Assert
        Assert.IsFalse(response.Success);
        Assert.AreEqual("Invalid quantity input!", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task ReduceQuantity_ReturnError_ExceptionOccurs()
    {
        // Arrange
        _mockSet = new Mock<DbSet<Medication>>();
        _mockContext = new Mock<MMContext>();
        _mockContext.Setup(m => m.Medications).Returns(_mockSet.Object);
        _service = new MedicationService(_mockContext.Object, _mapper.Object);

        var medicationId = Guid.NewGuid();
        var medication = new Medication { MedicationId = medicationId, Quantity = 10 };

        _mockContext.Setup(m => m.Medications.FindAsync(medicationId))
            .ReturnsAsync(medication);
        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var response = await _service.ReduceQuantity(medicationId, 5);

        // Assert
        Assert.IsFalse(response.Success);
        Assert.AreEqual("Error occurred while reducing medication's quantity.", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task ReduceQuantity_ReduceQuantity_NewQuantityIsValid()
    {
        // Arrange
        var medicationId = Guid.NewGuid();
        var medication = new Medication { MedicationId = medicationId,MedicationName = "Med1", Quantity = 10 };

        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        const int newQuantity = 5;

        // Act
        var response = await _service.ReduceQuantity(medicationId, newQuantity);

        // Assert
        Assert.IsTrue(response.Success);
        Assert.AreEqual(newQuantity, _context.Medications.First(m => m.MedicationId == medicationId).Quantity);
    }

}