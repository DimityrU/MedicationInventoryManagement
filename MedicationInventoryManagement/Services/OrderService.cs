using AutoMapper;
using MedicationInventoryManagement.Contracts;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Services;

public class OrderService : IOrderService
{
    private readonly MMContext _context;
    private readonly IMapper _mapper;

    public OrderService(MMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> PlaceOrder(OrderDTO orderDTO)
    {
        var response = new BaseResponse();
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orderData = await GenerateOrderData();
            var order = _mapper.Map<Order>(orderData);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderMedications = new List<OrderMedication>();
            foreach (var orderMedicationDto in orderDTO.OrderMedications)
            {
                var orderMedication = await HandleMedication(order, orderMedicationDto);
                orderMedications.Add(orderMedication);
            }

            await _context.OrderMedications.AddRangeAsync(orderMedications);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            response.AddError("SystemError - Cannot submit the order!");
        }

        return response;
    }

    public async Task<AllOrdersResponse> GetAllShippedOrders()
    {
        var response = new AllOrdersResponse();
        try
        {
            var orders = await _context.Orders.Where(o => o.Status == "shipped").Include(o =>o.OrderMedications).ToListAsync();

            if (orders == null)
            {
                response.AddError("Problem with getting the notification! Please try again!");
            }

            var ordersResponse = new List<OrderDTO>();
            foreach (var order in orders)
            {
                ordersResponse.Add(_mapper.Map<OrderDTO>(order));
            }

            response.Orders = ordersResponse;
        }
        catch (Exception)
        {
            response.AddError("Problem with getting the notification! Please try again!");
        }

        return response;
    }

    private async Task<OrderMedication> HandleMedication(Order order, OrderMedicationDTO orderMedicationDto)
    {
        var medication = orderMedicationDto.Medication;

        if (medication.MedicationName != null)
        {
            medication.MedicationId = Guid.Empty;
            var newMedication = _mapper.Map<Medication>(medication);

            _context.Medications.Add(newMedication);
            await _context.SaveChangesAsync();

            return new OrderMedication { OrderId = order.OrderId, MedicationId = newMedication.MedicationId, NewQuantity = orderMedicationDto.NewQuantity };
        }
        else
        {
            var oldMedication = _mapper.Map<Medication>(medication);
            return new OrderMedication { OrderId = order.OrderId, MedicationId = oldMedication.MedicationId, NewQuantity = orderMedicationDto.NewQuantity };
        }
    }

    private async Task<OrderDTO> GenerateOrderData()
    {
        var orderCheck = true;
        var orderName = "";
        var orderNumber = _context.Orders.Count() + 1;
        while (orderCheck)
        {
            orderName = "ORD-" + orderNumber.ToString("D4");
            orderCheck = _context.Orders.Any(o => o.OrderName == orderName);
            orderNumber++;
        }

        return new OrderDTO { OrderName = orderName, OrderDate = DateTime.Now, Status = "shipped" };
    }
}