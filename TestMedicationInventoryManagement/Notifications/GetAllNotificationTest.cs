﻿using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestMedicationInventoryManagement.Notifications;

[TestClass]
public class GetAllNotificationTest
{
    private MMContext _context;
    private INotificationsService _service;
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
    public async Task GetAllNotifications_AllNotifications_NotificationsExistInDatabase()
    {
        // Arrange
        await SetupTestDataAsync();

        _mapper.Setup(m => m.Map<NotificationDTO>(It.IsAny<Notification>())).Returns(new NotificationDTO());

        // Act
        var result = await _service.GetAllNotifications();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(7, result.Notifications.Count);
    }

    [TestMethod]
    public async Task GetAllNotifications_ReturnsError_ExceptionThrown()
    {
        // Arrange
        await SetupTestDataAsync();

        _mapper.Setup(m => m.Map<NotificationDTO>(It.IsAny<Notification>())).Throws(new Exception());

        // Act
        var result = await _service.GetAllNotifications();

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(result.Errors.FirstOrDefault().ErrorMessage , "Problem with getting the notification! Please try again!");
    }

    [TestMethod]
    public async Task GetAllNotifications_EmptyList_NoNotificationsExistInDatabase()
    {
        // Arrange

        // Act
        var result = await _service.GetAllNotifications();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Notifications);
        Assert.AreEqual(0, result.Notifications.Count);
    }
}