using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Services;

namespace TestMedicationInventoryManagement.LogIn
{
    [TestClass]
    public class LogInServiceTests
    {
        [TestMethod]
        public async Task ValidateUser_ValidCredentials_ReturnsTrue()
        {
            var logInService = new LogInService(new MMContext());

            var result = await logInService.ValidateUser("DimityrU", "userMM23");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateUser_InvalidUsername_ReturnsFalse()
        {
            var logInService = new LogInService(new MMContext());

            var result = await logInService.ValidateUser("username", "userMM23");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateUser_InvalidPassword_ReturnsFalse()
        {
            var logInService = new LogInService(new MMContext());
            
            var result = await logInService.ValidateUser("DimityrU", "user123");

            Assert.IsFalse(result);
        }
    }
}