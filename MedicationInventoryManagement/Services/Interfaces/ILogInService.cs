namespace MedicationInventoryManagement.Services.Interfaces;

public interface ILogInService
{
    Task<bool> ValidateUser(string username, string password);
}