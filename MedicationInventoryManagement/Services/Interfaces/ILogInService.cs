namespace MedicationInventoryManagement.Services.Interfaces;

public interface ILogInService
{
    bool ValidateUser(string username, string password);
}