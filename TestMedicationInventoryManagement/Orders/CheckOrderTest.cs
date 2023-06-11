using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Orders;

[TestClass]
public class CheckOrderTest
{
    private MMContext _context;
    private IOrderService _service;
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

        _service = new OrderService(_context, _mapper.Object);
    }

    [TestMethod]
    public async Task CheckOrder_True_OrderExist()
    {
        // Arrange
        var medicationId = Guid.NewGuid();
        var orderMedication = new OrderMedication { MedicationId = medicationId };
        _context.OrderMedications.Add(orderMedication);
        await _context.SaveChangesAsync();

        // Act
        var result = _service.CheckOrder(medicationId);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckOrder_False_OrderDoesNotExist()
    {
        // Arrange
        var medicationId = Guid.NewGuid();

        // Act
        var result = _service.CheckOrder(medicationId);

        // Assert
        Assert.IsFalse(result);
    }

}