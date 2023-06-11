using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Orders;

[TestClass]
public class CancelOrderTest
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

    //Transactions are not supported by the in-memory store. No unit tests for now
}