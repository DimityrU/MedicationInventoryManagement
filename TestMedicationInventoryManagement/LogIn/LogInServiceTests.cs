using MedicationInventoryManagement.Services;

namespace TestMedicationInventoryManagement.LogIn
{
    [TestClass]
    public class LogInServiceTests
    {
        [TestMethod]
        public void ValidateUser_ValidCredentials_ReturnsTrue()
        {
            var logInService = new LogInService();

            bool result = logInService.ValidateUser("DimityrU", "userMM23");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateUser_InvalidUsername_ReturnsFalse()
        {
            var logInService = new LogInService();

            bool result = logInService.ValidateUser("username", "userMM23");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateUser_InvalidPassword_ReturnsFalse()
        {
            var logInService = new LogInService();
            
            bool result = logInService.ValidateUser("DimityrU", "user123");

            Assert.IsFalse(result);
        }
    }
}