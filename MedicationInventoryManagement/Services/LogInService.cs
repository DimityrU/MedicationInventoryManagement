using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;

namespace MedicationInventoryManagement.Services;

public class LogInService : ILogInService
{
    public bool ValidateUser(string username, string password)
    {
        var user = new User();

        using (MMContext mmContext = new MMContext())
        {
            user = mmContext.Users.FirstOrDefault(u => u.UserName == username);
        }

        if (user != null)
        {
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (isPasswordValid)
                return true;

        }

        return false;
    }
}