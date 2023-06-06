using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicationInventoryManagement.Services;

public class LogInService : ILogInService
{
    private readonly MMContext _context;

    public LogInService(MMContext mmContext)
    {
        _context = mmContext;
    }


    public async Task<bool> ValidateUser(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null) return false;
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

        return isPasswordValid;
    }
}