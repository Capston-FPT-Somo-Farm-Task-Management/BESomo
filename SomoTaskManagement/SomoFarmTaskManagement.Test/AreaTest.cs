using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Farm;
using SomoTaskManagement.Domain.Model.Zone;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoFarmTaskManagement.Test
{
    [TestFixture]
    public class AreaTest
    {
        private Mock<IAreaService> _areaServiceMock;
        private Mock<IFarmService> _farmServiceMock;
        private Mock<IZoneService> _zoneServiceMock;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _areaServiceMock = new Mock<IAreaService>();
            _farmServiceMock = new Mock<IFarmService>();
            _zoneServiceMock = new Mock<IZoneService>();
        }

        //Test list all area
        [Test]
        public async Task ListArea_Should_Return_AreaModel()
        {
            _areaServiceMock.Setup(areaService => areaService.ListArea())
               .ReturnsAsync(new List<AreaModel> { new AreaModel() });

            var areas = await _areaServiceMock.Object.ListArea();

            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<AreaModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
            Assert.IsTrue(areas.Any(), "Danh sách areas phải chứa ít nhất một phần tử");
        }

        //Test list all area active
        [Test]
        public async Task ListAreaActive_Should_Return_AreaModel()
        {
            _areaServiceMock.Setup(areaService => areaService.ListAreaActive())
               .ReturnsAsync(new List<AreaModel> { new AreaModel() });

            var areas = await _areaServiceMock.Object.ListAreaActive();

            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<AreaModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
            Assert.IsTrue(areas.Any(), "Danh sách areas phải chứa ít nhất một phần tử");
        }

        //Test list active area by farm
        [Test]
        public async Task GetAreaByFarmId_All_Should_Return_AreaModelList()
        {

            _areaServiceMock.Setup(areaService => areaService.GetAreaByFarmId(It.IsAny<int>()))
               .ReturnsAsync((int farmId) => new List<AreaModel> { new AreaModel() });
            // Arrange
            int farmId = 1;

            // Act
            var areas = await _areaServiceMock.Object.GetAreaByFarmId(farmId);

            // Assert
            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<AreaModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
        }

        //Test list active area by farm fail (Error: can not find farmId)
        [Test]
        public async Task GetAreaByFarmId_All_Should_Return_Exception()
        {
            int farmId = 1;

            _farmServiceMock.Setup(farmServiceMock => farmServiceMock.GetFarmById(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy nông trại"));
            Exception exception = null;
            try
            {
                var farm = await _farmServiceMock.Object.GetFarmById(farmId);
                await _areaServiceMock.Object.GetAreaByFarm(farm.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy nông trại", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }


        //Test list all area by farm
        [Test]
        public async Task GetAreaByFarmId_Active_Should_Return_AreaModelList()
        {
            _areaServiceMock.Setup(areaService => areaService.GetAllAreaByFarmId(It.IsAny<int>()))
              .ReturnsAsync((int farmId) => new List<AreaModel> { new AreaModel() });

            // Arrange
            int farmId = 1;

            // Act
            var areas = await _areaServiceMock.Object.GetAllAreaByFarmId(farmId);

            // Assert
            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<AreaModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
        }

        //Test get area by id fail (Error:Can not find farmId)
        [Test]
        public async Task GetAreaByFarmId_Active_Should_Return_Exception()
        {
            int farmId = 1;

            _farmServiceMock.Setup(farmServiceMock => farmServiceMock.GetFarmById(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy nông trại"));
            Exception exception = null;
            try
            {
                var farm = await _farmServiceMock.Object.GetFarmById(farmId);
                await _areaServiceMock.Object.GetAllAreaByFarmId(farm.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy nông trại", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }



        //Test get area by id cuccess
        [Test]
        public async Task GetFarmById_Should_Return_Area()
        {
            _areaServiceMock.Setup(areaService => areaService.GetArea(It.IsAny<int>()))
             .ReturnsAsync((int areaId) => new AreaModel { Id = areaId, Name = "Area test" });

            int areaId = 1;
            var expectedArea = new AreaModel { Id = areaId, Name = "Area test" };

            var area = await _areaServiceMock.Object.GetArea(areaId);

            Assert.IsNotNull(area, "Area không được null");
            Assert.AreEqual(expectedArea.Id, area.Id, "Id của Area không khớp");
        }

        //Test get area by id fail (Error:Can not find AreaId)
        [Test]
        public async Task GetFarmById_Should_Return_Exception()
        {
            _areaServiceMock.Setup(areaService => areaService.GetArea(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Vùng không tìm thấy"));

            int areaId = 2;
            try
            {
                await _areaServiceMock.Object.GetArea(areaId);

                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Vùng không tìm thấy", ex.Message);
            }
        }


        //Test create area success
        [Test]
        public async Task CreateArea_Should_Call_AddArea_Method_In_AreaService()
        {
            var areaCreateUpdateModel = new AreaCreateUpdateModel
            {
                Name = "Test area",
                Code = "TEST",
                FArea = 1000,
                FarmId = 1,
            };
            await _areaServiceMock.Object.AddArea(areaCreateUpdateModel);

            _areaServiceMock.Verify(f => f.AddArea(It.Is<AreaCreateUpdateModel>(f => f == areaCreateUpdateModel)), Times.Once);
        }


        //Test update area success
        [Test]
        public async Task UpdateArea_Should_Call_UpdateArea_In_AreaService()
        {
            // Arrange
            int areaId = 1;
            var areaCreateUpdateModel = new AreaCreateUpdateModel
            {
                Name = "Test area",
                Code = "TEST",
                FArea = 1000,
                FarmId = 1,
            };

            var existingArea = new AreaModel
            {
                Id = areaId,
                Name = "Original Name",
                Code = "ORIGINAL",
                FArea = 2000,
                Status = "Active",
                FarmName = "Bao Loc Farm",
            };

            _areaServiceMock.Setup(u => u.GetArea(areaId)).ReturnsAsync(existingArea);

            // Act
            await _areaServiceMock.Object.UpdateArea(areaId, areaCreateUpdateModel);

            // Assert
            _areaServiceMock.Verify(u => u.UpdateArea(areaId, areaCreateUpdateModel), Times.Once);

            Assert.AreEqual("Original Name", existingArea.Name);
            Assert.AreEqual("Active", existingArea.Status);
            Assert.AreEqual("ORIGINAL", existingArea.Code);
            Assert.AreEqual(2000, existingArea.FArea);
            Assert.AreEqual("Bao Loc Farm", existingArea.FarmName);
        }

        // Test update area fail (Error: can not find areaId)
        [Test]
        public async Task UpdateFarm_With_InvalidId_Should_Throw_Exception()
        {
            // Arrange
            int areaId = 1;
            var areaCreateUpdateModel = new AreaCreateUpdateModel
            {
                Name = "Test area",
                Code = "TEST",
                FArea = 1000,
                FarmId = 1,
            };

            var existingArea = new AreaModel
            {
                Id = 2,
                Name = "Original Name",
                Code = "ORIGINAL",
                FArea = 2000,
                Status = "Active",
                FarmName = "Bao Loc Farm",
            };

            _areaServiceMock.Setup(u => u.GetArea(areaId)).ReturnsAsync(existingArea);

            _areaServiceMock.Setup(u => u.UpdateArea(areaId, areaCreateUpdateModel))
                .ThrowsAsync(new Exception("Không tìm thấy trang trại"));

            Assert.ThrowsAsync<Exception>(() => _areaServiceMock.Object.UpdateArea(areaId, areaCreateUpdateModel), "Không tìm thấy trang trại");
        }

        //Delete area success
        [Test]
        public async Task DeleteArea_WithValidAreaId_ShouldDeleteArea()
        {
            // Arrange
            _areaServiceMock.Setup(areaService => areaService.GetArea(It.IsAny<int>()))
             .ReturnsAsync((int areaId) => new AreaModel { Id = 1, Name = "Area test" });

            _zoneServiceMock.Setup(zoneService => zoneService.GetByArea(It.IsAny<int>()))
                .ReturnsAsync(new List<ZoneModel>());

            // Act
            await _areaServiceMock.Object.DeleteArea(1);

            // Assert
            _areaServiceMock.Verify(a => a.DeleteArea(1), Times.Once);

            //_areaServiceMock.Verify(a => a.DeleteArea.Commit(), Times.Once);
        }

        //Delete area fail (Error can not find areaId)
        [Test]
        public void DeleteArea_WithNonexistentArea_ShouldThrowException()
        {
            // Arrange
            int areaId = 1;
            _areaServiceMock.Setup(u => u.DeleteArea(areaId))
                .ThrowsAsync(new Exception("Không tìm thấy khu vực"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _areaServiceMock.Object.DeleteArea(areaId), "Không tìm thấy khu vực");
        }

        //Delete area fail (Error have the entity inside)
        [Test]
        public async Task DeleteArea_HaveTheEntityInside_ShouldThrowException()
        {
            // Arrange
            _areaServiceMock.Setup(areaService => areaService.GetArea(It.IsAny<int>()))
                .ReturnsAsync((int areaId) => new AreaModel { Id = 1, Name = "Area test" });

            _zoneServiceMock.Setup(zoneService => zoneService.ListActiveZone())
                .ReturnsAsync(new List<ZoneModel> { new ZoneModel() }); 

            _areaServiceMock.Setup(areaService => areaService.DeleteArea(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Không thể xóa vùng này khi còn thực thể bên trong"));

            // Act & Assert
            Exception exception = null;
            try
            {
                await _areaServiceMock.Object.DeleteArea(1);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Verify that DeleteArea was called
            _areaServiceMock.Verify(a => a.DeleteArea(1), Times.Once);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual("Không thể xóa vùng này khi còn thực thể bên trong", exception.Message);
        }
    }
}

