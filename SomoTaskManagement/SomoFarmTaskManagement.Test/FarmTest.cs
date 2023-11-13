using NUnit.Framework;
using SomoTaskManagement.Services.Interface;
using Moq;
using SomoTaskManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Data;
using Microsoft.AspNetCore.Http;
using SomoTaskManagement.Domain.Model.Farm;

namespace SomoFarmTaskManagement.Test
{
    [TestFixture]
    public class FarmServiceTests
    {
        private Mock<IFarmService> _farmServiceMock;

        [SetUp]
        public void Setup()
        {
            _farmServiceMock = new Mock<IFarmService>();

            _farmServiceMock.Setup(farmService => farmService.ListFarm())
                .ReturnsAsync(new List<Farm> { new Farm() });

            _farmServiceMock.Setup(farmService => farmService.GetFarmById(It.IsAny<int>()))
                .ReturnsAsync((int farmId) => new Farm { Id = farmId, Name = "Test Farm" });
        }

        [Test]
        public async Task ListFarm_Should_Return_Farms()
        {
            var farms = await _farmServiceMock.Object.ListFarm();

            Assert.IsNotNull(farms, "Danh sách farms không được null");
            Assert.IsInstanceOf<IEnumerable<Farm>>(farms, "Danh sách farms phải là một IEnumerable<Farm>");
            Assert.IsTrue(farms.Any(), "Danh sách farms phải chứa ít nhất một phần tử");
        }

        [Test]
        public async Task GetFarmById_Should_Return_Farm()
        {
            int farmId = 1;
            var expectedFarm = new Farm { Id = farmId, Name = "Test Farm" };

            var farm = await _farmServiceMock.Object.GetFarmById(farmId);

            Assert.IsNotNull(farm, "Farm không được null");
            Assert.AreEqual(expectedFarm.Id, farm.Id, "Id của Farm không khớp");
        }

        [Test]
        public async Task CreateFarm_Should_Call_CreateFarm_Method_In_FarmService()
        {
            var farmCreateUpdateModel = new FarmCreateUpdateModel
            {
                Name = "Test Farm",
                FarmArea = 10,
                Address = "Test",
                Description = "Test",
                ImageFile = new Mock<IFormFile>().Object
            };
            await _farmServiceMock.Object.CreateFarm(farmCreateUpdateModel);

            _farmServiceMock.Verify(f => f.CreateFarm(It.Is<FarmCreateUpdateModel>(f => f == farmCreateUpdateModel)), Times.Once);
        }

        [Test]
        public async Task UpdateFarm_Should_Update_Farm_And_UploadImage()
        {
            // Arrange
            int farmId = 1;
            var farmCreateUpdateModel = new FarmCreateUpdateModel
            {
                Name = "Updated Farm",
                Description = "Updated Description",
                FarmArea = 10,
                Address = "Update Address",
                //ImageFile = "string"
            };

            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Updated Farm",
                Description = "Updated Description",
                FarmArea = 10,
                Address = "Original Address",
                Status = 1,
                UrlImage = "string"
            };

            var imageFileMock = new Mock<IFormFile>().Object;

            _farmServiceMock.Setup(u => u.GetFarmById(farmId)).ReturnsAsync(existingFarm);

            // Act
            await _farmServiceMock.Object.UpdateFarm(farmId, farmCreateUpdateModel);

            // Assert
            _farmServiceMock.Verify(u => u.UpdateFarm(farmId, farmCreateUpdateModel), Times.Once);

            Assert.AreEqual("Updated Farm", existingFarm.Name);
            Assert.AreEqual(1, existingFarm.Status);
            Assert.AreEqual("Updated Description", existingFarm.Description);
            Assert.AreEqual("Original Address", existingFarm.Address);
            Assert.AreEqual(10, existingFarm.FarmArea);

            //_firebaseStorageMock.Verify(f => f.PutAsync(It.IsAny<Stream>()), Times.Once);
            //_firebaseStorageMock.Verify(f => f.GetDownloadUrlAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateFarm_With_InvalidId_Should_Throw_Exception()
        {
            // Arrange
            int farmId = 1; 
            var farmCreateUpdateModel = new FarmCreateUpdateModel
            {
                Name = "Updated Farm",
                Description = "Updated Description",
                FarmArea = 10,
                Address = "Updated Address",
                //ImageFile = "string"
                // Các thuộc tính khác của FarmCreateUpdateModel
            };

            var existingFarm = new Farm
            {
                Id = 2,
                Name = "Updated Farm",
                Description = "Updated Description",
                FarmArea = 10,
                Address = "Updated Address",
                Status = 1,
                UrlImage = "string"
            };

            _farmServiceMock.Setup(u => u.GetFarmById(farmId)).ReturnsAsync(existingFarm);

            _farmServiceMock.Setup(u => u.UpdateFarm(farmId, farmCreateUpdateModel))
                .ThrowsAsync(new Exception("Không tìm thấy trang trại"));

            Assert.ThrowsAsync<Exception>(() => _farmServiceMock.Object.UpdateFarm(farmId, farmCreateUpdateModel), "Không tìm thấy trang trại");
        }

    }

}
