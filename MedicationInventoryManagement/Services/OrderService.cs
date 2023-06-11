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
    
    public async Task<OrderDTO> GetOrder(Guid id)
    {
        var response = new OrderDTO();
        try
        {
            var order = await _context.Orders.Where(o => o.OrderId == id)
                .Include(o => o.OrderMedications)
                .ThenInclude(o => o.Medication)
                .FirstOrDefaultAsync();

            response =_mapper.Map<OrderDTO>(order);
        }
        catch (Exception)
        {
            return null;
        }

        return response;
    }

    public bool CheckOrder(Guid id)
    {
        return _context.OrderMedications.Any(o => o.MedicationId == id);
    }

    public async Task<BaseResponse> CancelOrder(Guid id)
    {
        var response = new BaseResponse();
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orderMedications = _context.OrderMedications.Include(o => o.Medication).Where(o => o.OrderId == id).ToList();

            if (orderMedications == null)
            {
                await transaction.RollbackAsync();
                response.AddError("Error occurred while canceling the order!");
                return response;

            }
            _context.OrderMedications.RemoveRange(orderMedications);
            await _context.SaveChangesAsync();

            var medications = new List<Medication>();
            foreach (var medication in orderMedications)
            {
                if (medication.Medication.ExpirationDate == null && medication.Medication.Quantity == null)
                {
                    medications.Add(medication.Medication);
                }
            }

            _context.Medications.RemoveRange(medications);
            await _context.SaveChangesAsync();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                await transaction.RollbackAsync();
                response.AddError("Error occurred while canceling the order!");
                return response;
            }

            order.Status = "cancel";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            response.AddError("Error occurred while canceling the order.");
        }

        return response;
    }

    public async Task<BaseResponse> FinishOrder(OrderDTO order)
    {
        var response = new BaseResponse();
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var updatedMedications = await UpdateMedication(order.OrderMedications);
            if (updatedMedications == null)
            {
                await transaction.RollbackAsync();
                response.AddError("Error occurred while finishing the order!");
                return response;
            }

            _context.Medications.UpdateRange(updatedMedications);
            await _context.SaveChangesAsync();

            var orderMedicationsToRemove = await _context.OrderMedications.Where(o => o.OrderId == order.OrderId).ToListAsync();
            if (orderMedicationsToRemove == null)
            {
                await transaction.RollbackAsync();
                response.AddError("Error occurred while finishing the order!");
                return response;
            }

            _context.OrderMedications.RemoveRange(orderMedicationsToRemove);
            await _context.SaveChangesAsync();
            
            var orderToUpdate = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            if (orderToUpdate == null)
            {
                await transaction.RollbackAsync();
                response.AddError("Error occurred while finishing the order!");
                return response;
            }
            orderToUpdate.Status = "arrived";
            _context.Orders.Update(orderToUpdate);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            response.AddError($"An error occurred: {e.Message}");
        }

        return response;
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

    private async Task<List<Medication>> UpdateMedication(List<OrderMedicationDTO> orderMedications)
    {
        var medicationsInDb = await _context.Medications.ToListAsync();
        var updatedMedications = new List<Medication>();

        foreach (var orderMedication in orderMedications)
        {
            var medicationInDb = medicationsInDb.FirstOrDefault(m => m.MedicationId == orderMedication.Medication.MedicationId);

            if (medicationInDb != null)
            {
                if (medicationInDb.Quantity == null)
                {
                    medicationInDb.Quantity = orderMedication.NewQuantity;
                    medicationInDb.ExpirationDate = orderMedication.Medication.ExpirationDate;
                }
                else
                {
                    medicationInDb.Quantity += orderMedication.NewQuantity;
                }
                updatedMedications.Add(medicationInDb);
            }
        }

        return updatedMedications;
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
}