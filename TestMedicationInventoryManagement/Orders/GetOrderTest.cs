using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MedicationInventoryManagement.Models;

namespace TestMedicationInventoryManagement.Orders;

[TestClass]
public class GetOrderTest
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

    private async Task SetupTestDataAsync()
    {
        var order1 = new Order { OrderId = Guid.NewGuid(), OrderName = "ORD-000Y", Status = "shipped" };
        var order2 = new Order { OrderId = Guid.NewGuid(), OrderName = "ORD-000X", Status = "shipped" };
        _context.Orders.Add(order1);
        _context.Orders.Add(order2);
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task GetOrder_Order_OrderExists()
    {
        // Arrange
        await SetupTestDataAsync();
        var orderId = _context.Orders.First().OrderId;
        var order = _context.Orders.First();

        var orderDto = new OrderDTO();

        _mapper.Setup(m => m.Map<OrderDTO>(order)).Returns(orderDto);

        // Act
        var result = await _service.GetOrder(orderId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orderDto, result);
    }

    [TestMethod]
    public async Task GetOrder_Null_OrderDoesNotExist()
    {
        // Arrange
        await SetupTestDataAsync();
        var nonExistentOrderId = Guid.NewGuid();

        // Act
        var result = await _service.GetOrder(nonExistentOrderId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetOrder_Null_ExceptionOccurs()
    {
        // Arrange
        await SetupTestDataAsync();
        var orderId = _context.Orders.First().OrderId;
        var order = _context.Orders.First();

        _mapper.Setup(m => m.Map<OrderDTO>(order)).Throws<Exception>();
        // Act
        var result = await _service.GetOrder(orderId);

        // Assert
        Assert.IsNull(result);
    }


}