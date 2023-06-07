using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;

namespace MedicationInventoryManagement.Services.Interfaces;

public interface INotificationsService
{
    Task<NotificationResponse> GetAllNotifications();

    Task<BaseResponse> CheckLowQuantityNotification(Guid? medicationId);

    public Task<BaseResponse> CheckExpirationDateNotification(Guid? medicationId);

    public Task DeleteNotification(Guid? medicationId, string type = "all");
}