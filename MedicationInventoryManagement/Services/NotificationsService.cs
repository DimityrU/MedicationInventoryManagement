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
}