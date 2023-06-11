using AutoMapper;
using MedicationInventoryManagement.Entities;
using MedicationInventoryManagement.Models;
using MedicationInventoryManagement.Services;
using MedicationInventoryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestMedicationInventoryManagement.Home;

[TestClass]
public class AddMedicationTest
{
    private Mock<MMContext>? _mockContext;
    private Mock<DbSet<Medication>>? _mockSet;
    private IMedicationService _service;
    private Mock<IMapper>? _mapper;

    [TestInitialize]
    public void SetUp()
    {
        _mockSet = new Mock<DbSet<Medication>>();
        _mockContext = new Mock<MMContext>();
        _mockContext.Setup(m => m.Medications).Returns(_mockSet.Object);
        _mapper = new Mock<IMapper>();

        _service = new MedicationService(_mockContext.Object, _mapper.Object);
    }

    [TestMethod]
    public async Task AddMedication_ReturnError_QuantityLessThanZero()
    {
        var medicationRequest = new MedicationDTO { Quantity = -1 };

        var response = await _service.AddMedication(medicationRequest);

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Medication quantity shouldn't be less than 0.", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task AddMedication_ReturnError_ExpirationDate()
    {
        var medicationRequest = new MedicationDTO { Quantity = 5, ExpirationDate = DateTime.Now.AddDays(15) };

        var response = await _service.AddMedication(medicationRequest);

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Medication is expired or date is too soon to be added to the system.", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task AddMedication_ReturnError_Exception()
    {
        var medicationRequest = new MedicationDTO { Quantity = 5, ExpirationDate = DateTime.Now.AddMonths(2) };

        _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        var response = await _service.AddMedication(medicationRequest);

        Assert.IsFalse(response.Success);
        Assert.AreEqual("Error occurred while adding medication.", response.Errors.First().ErrorMessage);
    }

    [TestMethod]
    public async Task AddMedication_ReturnSuccess_RequestIsValid()
    {
        var medicationRequest = new MedicationDTO { Quantity = 5, ExpirationDate = DateTime.Now.AddMonths(2) };

        var response = await _service.AddMedication(medicationRequest);

        Assert.IsTrue(response.Success);
        _mockSet.Verify(m => m.AddAsync(It.IsAny<Medication>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}