using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Notifications;

[TestClass]
public class CheckExpirationDateNotificationTest
{
    private MMContext _context;
    private NotificationsService _service;
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

        _service = new NotificationsService(_context, _mapper.Object);
    }

    private async Task SetupTestDataAsync()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        var guid4 = Guid.NewGuid();
        var guid5 = Guid.NewGuid();
        var guid6 = Guid.NewGuid();
        var guid7 = Guid.NewGuid();

        var medications = new List<Medication>
    {
        new() { MedicationId = guid1, MedicationName = "Med1", ExpirationDate = DateTime.Now.AddDays(15), Quantity = 5 },
        new() { MedicationId = guid2, MedicationName = "Med2", ExpirationDate = DateTime.Now.AddDays(-10), Quantity = 20 },
        new() { MedicationId = guid3, MedicationName = "Med3", ExpirationDate = DateTime.Now.AddMonths(2), Quantity = 30 },
        new() { MedicationId = guid4, MedicationName = "Med4", ExpirationDate = DateTime.Now.AddDays(-10), Quantity = 5 },
        new() { MedicationId = guid5, MedicationName = "Med5", ExpirationDate = DateTime.Now.AddDays(15), Quantity = 50 },
        new() { MedicationId = guid6, MedicationName = "Med6", ExpirationDate = DateTime.Now.AddMonths(2), Quantity = 2 },
        new() { MedicationId = guid7, MedicationName = "Med7", ExpirationDate = DateTime.Now.AddMonths(2), Quantity = 30 }
    };
        await _context.Medications.AddRangeAsync(medications);

        var notifications = new List<Notification>
    {
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med1 is almost out!", NotificationType = "low quantity", MedicationId = guid1},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med2 is expired!", NotificationType = "expired", MedicationId = guid2},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med1 is expiring in less than a month!", NotificationType = "expiring", MedicationId = guid1},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med4 is almost out!", NotificationType = "low quantity", MedicationId = guid4},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med4 is expired!", NotificationType = "expired", MedicationId = guid4},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med6 is almost out!", NotificationType = "low quantity", MedicationId = guid6},
        new() { NotificationId = Guid.NewGuid(), NotificationMessage = "Med5 is expiring in less than a month!", NotificationType = "expiring", MedicationId = guid5}
    };
        await _context.Notifications.AddRangeAsync(notifications);

        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task CheckExpirationDateNotification_ReturnsError_WhenMedicationDoesNotExist()
    {
        // Arrange
        var randomGuid = Guid.NewGuid();

        // Act
        var result = await _service.CheckExpirationDateNotification(randomGuid);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(result.Errors.FirstOrDefault().ErrorMessage, "Cannot check for expiration date!");
    }

    [TestMethod]
    public async Task CheckExpirationDateNotification_GeneratesExpiringNotification_ExpirationDateLessThan30Days()
    {
        // Arrange
        await SetupTestDataAsync();
        var guid = _context.Medications.First(m => m.MedicationName == "Med1").MedicationId;

        // Act
        var result = await _service.CheckExpirationDateNotification(guid);

        // Assert
        Assert.IsTrue(result.Success);
        var notification = _context.Notifications.FirstOrDefault(n => n.MedicationId == guid && n.NotificationType == "expiring");
        Assert.IsNotNull(notification);
        Assert.AreEqual(notification.NotificationMessage, "Med1 is expiring in less than a month!");
    }

    [TestMethod]
    public async Task CheckExpirationDateNotification_GeneratesExpiredNotification_ExpirationDatePassed()
    {
        // Arrange
        await SetupTestDataAsync();
        var guid = _context.Medications.First(m => m.MedicationName == "Med2").MedicationId;

        // Act
        var result = await _service.CheckExpirationDateNotification(guid);

        // Assert
        Assert.IsTrue(result.Success);
        var notification = _context.Notifications.FirstOrDefault(n => n.MedicationId == guid && n.NotificationType == "expired");
        Assert.IsNotNull(notification);
        Assert.AreEqual(notification.NotificationMessage, "Med2 is expired!");
    }

    [TestMethod]
    public async Task CheckExpirationDateNotification_DoesNotGenerateNotification_ExpirationDateMoreThan30Days()
    {
        // Arrange
        await SetupTestDataAsync();
        var guid = _context.Medications.First(m => m.MedicationName == "Med3").MedicationId;

        // Act
        var result = await _service.CheckExpirationDateNotification(guid);

        // Assert
        Assert.IsTrue(result.Success);
        var notification = _context.Notifications.FirstOrDefault(n => n.MedicationId == guid);
        Assert.IsNull(notification);
    }



}