using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface INotificationsService
{
    Task<NotificationResponse> GetAllNotifications();

    // Generate Notification

    //Remove Notification

    //MarkAsSeen Notifications
}