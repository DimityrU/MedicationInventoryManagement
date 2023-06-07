using AutoMapper;
using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Services;

public class NotificationsService : INotificationsService
{
    private readonly MMContext _context;
    private readonly IMapper _mapper;

    public NotificationsService(MMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<NotificationResponse> GetAllNotifications()
    {
        var response = new NotificationResponse();
        try
        {
            var notifications = await _context.Notifications.Include(n => n.Medication).ToListAsync();

            if (notifications == null)
            {
                response.AddError("Problem with getting the notification! Please try again!");
            }

            var notificationsResponse = new List<NotificationDTO>();
            foreach (var notification in notifications)
            {
                notificationsResponse.Add(_mapper.Map<NotificationDTO>(notification));
            }

            response.Notifications = notificationsResponse;
        }
        catch (Exception )
        {
            response.AddError("Problem with getting the notification! Please try again!");
        }

        return response;
    }

    public async Task<BaseResponse> CheckLowQuantityNotification(Guid? medicationId)
    {
        const int minQuantity = 10;
        const string type = "low quantity";
        var response = new BaseResponse();
        try
        {
            if (await NotificationExist(medicationId, type))
            {
                return response;
            }
            var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == medicationId);

            if (medication == null)
            {
                response.AddError("Cannot check for Quantity!");
            }

            string message = $"{medication.MedicationName} is almost out.";

            if (medication.Quantity <= minQuantity)
            {
                await GenerateNotification(type, message, medication);
            }
        }
        catch (Exception)
        {
            response.AddError("Server error, please refresh!");
        }

        return response;
    }

    public async Task<BaseResponse> CheckExpirationDateNotification(Guid? medicationId)
    {
        const string typeExpiring = "expiring";
        const string typeExpired = "expired";
        var response = new BaseResponse();
        try
        {
            if (await NotificationExist(medicationId, typeExpiring) && await NotificationExist(medicationId, typeExpired))
            {
                return response;
            }
            var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == medicationId);

            if (medication == null)
            {
                response.AddError("Cannot check for expiration date!");
            }
            string expiringMessage = $"{medication.MedicationName} is expiring in less than a month!";
            string expiredMessage = $"{medication.MedicationName} is expired!";

            if (medication.ExpirationDate  < DateTime.Now.Date.AddDays(30))
            {
                await GenerateNotification(typeExpiring, expiringMessage, medication);
            }
            else if(medication.ExpirationDate <= DateTime.Now.Date)
            {
                if (await NotificationExist(medicationId, typeExpiring))
                {
                    await DeleteNotification(medicationId, typeExpired);
                }

                await GenerateNotification(typeExpiring, expiredMessage, medication);
            }
        }
        catch (Exception)
        {
            response.AddError("Server error, please refresh!");
        }

        return response;
    }

    private async Task GenerateNotification(string typeExpiring, string expiringMessage, Medication medication)
    {
        var notification = new Notification()
        {
            NotificationType = typeExpiring,
            CreatedAt = DateTime.Now.Date,
            NotificationMessage = expiringMessage,
            Medication = medication
        };
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> NotificationExist(Guid? medicationId, string type)
    {
        var notification = await _context.Notifications
            .Where(n => n.MedicationId == medicationId 
                        && n.NotificationType == type).FirstOrDefaultAsync();

            return notification != null;
    }

    public async Task DeleteNotification(Guid? medicationId, string type = "all")
    {

        if (type == "all")
        {
            var notifications = await _context.Notifications
                .Where(n => n.MedicationId == medicationId).ToListAsync();

            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    _context.Notifications.Remove(notification);
                }
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            var notification = await _context.Notifications
                .Where(n => n.MedicationId == medicationId
                            && n.NotificationType == type).FirstOrDefaultAsync();
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}