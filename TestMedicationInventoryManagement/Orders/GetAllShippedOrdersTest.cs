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
public class GetAllShippedOrdersTest
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
    public async Task GetAllShippedOrders_ReturnAllShippedOrders()
    {
        // Arrange
        await SetupTestDataAsync();

        var orderDto1 = new OrderDTO();
        var orderDto2 = new OrderDTO();

        _mapper.Setup(m => m.Map<OrderDTO>(_context.Orders.First(o => o.OrderName == "ORD-000Y"))).Returns(orderDto1);
        _mapper.Setup(m => m.Map<OrderDTO>(_context.Orders.First(o => o.OrderName == "ORD-000X"))).Returns(orderDto2);

        // Act
        var response = await _service.GetAllShippedOrders();

        // Assert
        Assert.AreEqual(2, response.Orders.Count);
        Assert.AreEqual(orderDto1, response.Orders[0]);
        Assert.AreEqual(orderDto2, response.Orders[1]);
    }

    [TestMethod]
    public async Task GetAllShippedOrders_ReturnError_ExceptionOccurs()
    {
        // Arrange
        await SetupTestDataAsync();
        _mapper.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>())).Throws<Exception>(); // Force an exception

        // Act
        var response = await _service.GetAllShippedOrders();

        // Assert
        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(e => e.ErrorMessage == "Problem with getting the notification! Please try again!"));
    }
}