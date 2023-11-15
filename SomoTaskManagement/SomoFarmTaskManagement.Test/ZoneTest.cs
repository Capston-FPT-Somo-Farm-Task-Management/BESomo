using Moq;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Field;
using SomoTaskManagement.Domain.Model.Zone;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SomoFarmTaskManagement.Test
{
    public class ZoneTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAreaService> _areaServiceMock;
        private Mock<IFarmService> _farmServiceMock;
        private Mock<IZoneService> _zoneServiceMock;
        private Mock<IFieldService> _fieldServiceMock;
        private Mock<IZoneTypeService> _zoneTypeServiceMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _areaServiceMock = new Mock<IAreaService>();
            _farmServiceMock = new Mock<IFarmService>();
            _zoneServiceMock = new Mock<IZoneService>();
            _fieldServiceMock = new Mock<IFieldService>();
            _zoneTypeServiceMock = new Mock<IZoneTypeService>();
        }

        //Test list all zone
        [Test]
        public async Task ListZone_Should_Return_ZoneModel()
        {
            _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.ListZone())
               .ReturnsAsync(new List<ZoneModel> { new ZoneModel() });

            var zones = await _zoneServiceMock.Object.ListZone();

            Assert.IsNotNull(zones, "Danh sách zones không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(zones, "Danh sách zones phải là một IEnumerable<ZoneModel>");
            Assert.IsTrue(zones.Any(), "Danh sách zones phải chứa ít nhất một phần tử");
        }

        //Test list all zone active
        [Test]
        public async Task ListZoneActive_Should_Return_ZoneModel()
        {
            _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.ListActiveZone())
               .ReturnsAsync(new List<ZoneModel> { new ZoneModel() });

            var zones = await _zoneServiceMock.Object.ListActiveZone();

            Assert.IsNotNull(zones, "Danh sách zones không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(zones, "Danh sách zones phải là một IEnumerable<ZoneModel>");
            Assert.IsTrue(zones.Any(), "Danh sách zones phải chứa ít nhất một phần tử");
        }

        //Test get zone by id cuccess
        [Test]
        public async Task GetFarmById_Should_Return_Area()
        {
            _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.GetZone(It.IsAny<int>()))
             .ReturnsAsync((int zoneId) => new ZoneModel { Id = zoneId, Name = "Zone test" });

            int zoneId = 1;
            var expectedZone = new ZoneModel { Id = zoneId, Name = "Area test" };

            var zone = await _zoneServiceMock.Object.GetZone(zoneId);

            Assert.IsNotNull(zone, "Area không được null");
            Assert.AreEqual(expectedZone.Id, zone.Id, "Id của Area không khớp");
        }

        //Test get zone by id fail (Error:Can not find ZoneId)
        [Test]
        public async Task GetFarmById_Should_Return_Exception()
        {
            _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.GetZone(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Vùng không tìm thấy"));

            int zoneId = 2;
            try
            {
                await _zoneServiceMock.Object.GetZone(zoneId);

                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Vùng không tìm thấy", ex.Message);
            }
        }

        //Test list zone by areaId and type of livestock success
        [Test]
        public async Task GetByAreaAndLivestock_Should_Return_ZoneModelList()
        {
            _zoneServiceMock.Setup(areaService => areaService.GetByAreaAndLivestock(It.IsAny<int>()))
              .ReturnsAsync((int areaId) => new List<ZoneModel> { new ZoneModel() });

            // Arrange
            int areaId = 1;

            // Act
            var zones = await _zoneServiceMock.Object.GetByAreaAndLivestock(areaId);

            // Assert
            Assert.IsNotNull(zones, "Danh sách zones không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(zones, "Danh sách zones phải là một IEnumerable<AreaModel>");
        }

        //Test list zone by areaId and type of livestock fail(Error can not find areaId)
        [Test]
        public async Task GetByAreaAndLivestock_WhenAreaNotFound_ShouldThrowException()
        {
            int areaId = 1;

            _areaServiceMock.Setup(a => a.GetArea(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy khu vực"));
            Exception exception = null;
            try
            {
                var area = await _areaServiceMock.Object.GetArea(areaId);
                await _zoneServiceMock.Object.GetByAreaAndLivestock(area.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy khu vực", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }

        //Test list zone by areaId and type of plant success
        [Test]
        public async Task GetByAreaAndPlant_Should_Return_ZoneModelList()
        {
            _zoneServiceMock.Setup(areaService => areaService.GetByAreaAndLivestock(It.IsAny<int>()))
              .ReturnsAsync((int areaId) => new List<ZoneModel> { new ZoneModel() });

            // Arrange
            int areaId = 1;

            // Act
            var zones = await _zoneServiceMock.Object.GetByAreaAndPlant(areaId);

            // Assert
            Assert.IsNotNull(zones, "Danh sách zones không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(zones, "Danh sách zones phải là một IEnumerable<AreaModel>");
        }

        //Test list zone by areaId and type of plant fail(Error can not find areaId)
        [Test]
        public async Task GetByAreaAndPlant_WhenAreaNotFound_ShouldThrowException()
        {
            int areaId = 1;

            _areaServiceMock.Setup(a => a.GetArea(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy khu vực"));
            Exception exception = null;
            try
            {
                var area = await _areaServiceMock.Object.GetArea(areaId);
                await _zoneServiceMock.Object.GetByAreaAndPlant(area.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy khu vực", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }

        //Test create zone success
        [Test]
        public async Task CreateZone_Should_Call_AddArea_Method_In_ZoneService()
        {
            var zoneCreateUpdateModel = new ZoneCreateUpdateModel
            {
                Name = "Test area",
                Code = "TEST",
                FarmArea = 1000,
                AreaId = 1,
                ZoneTypeId = 2,
            };

            await _zoneServiceMock.Object.AddZone(zoneCreateUpdateModel);

            _zoneServiceMock.Verify(f => f.AddZone(It.Is<ZoneCreateUpdateModel>(f => f == zoneCreateUpdateModel)), Times.Once);
        }


        //Test list all zone by farm
        [Test]
        public async Task GetZoneByFarmId_Active_Should_Return_AreaModelList()
        {
            _zoneServiceMock.Setup(zoneService => zoneService.GetActiveByFarmId(It.IsAny<int>()))
              .ReturnsAsync((int farmId) => new List<ZoneModel> { new ZoneModel() });

            // Arrange
            int farmId = 1;

            // Act
            var areas = await _zoneServiceMock.Object.GetActiveByFarmId(farmId);

            // Assert
            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
        }

        //Test get zone by id fail (Error:Can not find farmId)
        [Test]
        public async Task GetZoneByFarmId_Active_Should_Return_Exception()
        {
            int farmId = 1;

            _farmServiceMock.Setup(farmServiceMock => farmServiceMock.GetFarmById(It.IsAny<int>()))
               .ThrowsAsync(new Exception("Không tìm thấy trang trại"));
            Exception exception = null;

            try
            {
                var farm = await _farmServiceMock.Object.GetFarmById(farmId);
                await _zoneServiceMock.Object.GetActiveByFarmId(farm.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy trang trại", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }


        //Test list all zone by taskTypeId
        [Test]
        public async Task GetByZoneTypeId_Should_Return_AreaModelList()
        {
            _zoneServiceMock.Setup(zoneService => zoneService.GetByZoneTypeId(It.IsAny<int>()))
              .ReturnsAsync((int farmId) => new List<ZoneModel> { new ZoneModel() });

            // Arrange
            int zoneTypeId = 1;

            // Act
            var areas = await _zoneServiceMock.Object.GetByZoneTypeId(zoneTypeId);

            // Assert
            Assert.IsNotNull(areas, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<ZoneModel>>(areas, "Danh sách areas phải là một IEnumerable<AreaModel>");
        }

        //Test get zone by id fail (Error:Can not find zoneTypeId)
        [Test]
        public async Task GetByZoneTypeId_Should_Return_Exception()
        {
            int zoneTypeId = 1;

            _zoneTypeServiceMock.Setup(zoneTypeServiceMock => zoneTypeServiceMock.GetZoneType(It.IsAny<int>()))
               .ThrowsAsync(new Exception("Không tìm thấy loại khu vùng"));
            Exception exception = null;

            try
            {
                var zoneType = await _zoneTypeServiceMock.Object.GetZoneType(zoneTypeId);
                await _zoneServiceMock.Object.GetByZoneTypeId(zoneType.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy loại khu vùng", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }

        //Test update zone success
        [Test]
        public async Task UpdateZone_Should_Call_UpdateZone_In_ZoneService()
        {
            // Arrange
            int zoneId = 1;
            var zoneCreateUpdateModel = new ZoneCreateUpdateModel
            {
                Name = "Test zone update",
                Code = "TEST",
                FarmArea = 1000,
                AreaId = 1,
                ZoneTypeId = 2,
            };

            var existingZone = new ZoneModel
            {
                Id = zoneId,
                Name = "Test zone",
                Code = "TEST",
                FarmArea = 1000,
                AreaId = 1,
                ZoneTypeId = 2,
            };

            _zoneServiceMock.Setup(u => u.GetZone(zoneId)).ReturnsAsync(existingZone);

            // Act
            await _zoneServiceMock.Object.UpdateZone(zoneId, zoneCreateUpdateModel);

            // Assert
            _zoneServiceMock.Verify(u => u.UpdateZone(zoneId, zoneCreateUpdateModel), Times.Once);

            Assert.AreEqual("Test zone", existingZone.Name);
            Assert.AreEqual("TEST", existingZone.Code);
            Assert.AreEqual(1000, existingZone.FarmArea);
            Assert.AreEqual(1, existingZone.AreaId);
            Assert.AreEqual(2, existingZone.ZoneTypeId);
        }

        // Test update zone fail (Error: can not find zoneId)
        [Test]
        public async Task UpdateFarm_With_InvalidId_Should_Throw_Exception()
        {
            // Arrange
            int zoneId = 1;
            var zoneCreateUpdateModel = new ZoneCreateUpdateModel
            {
                Name = "Test zone update",
                Code = "TEST",
                FarmArea = 1000,
                AreaId = 1,
                ZoneTypeId = 2,
            };

            var existingZone = new ZoneModel
            {
                Id = zoneId,
                Name = "Test zone",
                Code = "TEST",
                FarmArea = 1000,
                AreaId = 1,
                ZoneTypeId = 2,
            };

            _zoneServiceMock.Setup(u => u.GetZone(zoneId)).ReturnsAsync(existingZone);

            _zoneServiceMock.Setup(u => u.UpdateZone(zoneId, zoneCreateUpdateModel))
                .ThrowsAsync(new Exception("Không tìm thấy vùng"));

            Assert.ThrowsAsync<Exception>(() => _zoneServiceMock.Object.UpdateZone(zoneId, zoneCreateUpdateModel), "Không tìm thấy vùng");
        }



        //Delete zone success
        [Test]
        public async Task DeleteZone_WithValidAreaId_ShouldDeleteZone()
        {
            // Arrange
            _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.GetZone(It.IsAny<int>()))
             .ReturnsAsync((int zoneId) => new ZoneModel { Id = zoneId, Name = "Zone test" });

            _zoneServiceMock.Setup(zoneService => zoneService.GetByArea(It.IsAny<int>()))
                .ReturnsAsync(new List<ZoneModel>());

            // Act
            await _areaServiceMock.Object.DeleteArea(1);

            // Assert
            _areaServiceMock.Verify(a => a.DeleteArea(1), Times.Once);

            //_areaServiceMock.Verify(a => a.DeleteArea.Commit(), Times.Once);
        }

        ///Delete zone fail (Error have the entify inside)
        [Test]
        public async Task DeleteZone_HaveTheEntityInside_ShouldThrowException()
        {
            // Arrange
            var zoneIdToDelete = 1;
            var fieldId = 1;

            // Mock for RepositoryField
            _fieldServiceMock.Setup(fieldService => fieldService.ListFieldActive())
                .ReturnsAsync(new List<FieldModel> { new FieldModel { Id = fieldId, ZoneId = zoneIdToDelete, Status = "Plant" } });

            // Mock for RepositoryZone
            var zones = new List<ZoneModel> { new ZoneModel { Id = zoneIdToDelete, Name = "Zone test" } };

            _zoneServiceMock.Setup(areaService => areaService.GetZone(It.IsAny<int>()))
                .ReturnsAsync((int areaId) => new ZoneModel { Id = zoneIdToDelete, Name = "Area test" });

            _zoneServiceMock.Setup(uow => uow.Delete(It.IsAny<int>()))
                .Callback<int>(zoneId =>
                {
                    var fieldsInZone = _fieldServiceMock.Object.ListFieldActive().Result.Count();

                    if (fieldsInZone > 0)
                    {
                        throw new Exception("Không thể xóa vùng này khi còn thực thể bên trong");
                    }

                    var deletedZone = zones.FirstOrDefault(zone => zone.Id == fieldId);
                    if (deletedZone != null)
                    {
                        zones.Remove(deletedZone);
                    }
                });

            // Act and Assert
            Exception exception = null;
            try
            {
                await _zoneServiceMock.Object.Delete(zoneIdToDelete);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception, "Exception should not be null");
            Assert.AreEqual("Không thể xóa vùng này khi còn thực thể bên trong", exception.Message);

            // Verify that the RepositoryField.GetData, RepositoryZone.GetData, and RepositoryZone.Delete were called
            _fieldServiceMock.Verify(uow => uow.ListFieldActive(), Times.Once);
            _zoneServiceMock.Verify(uow => uow.Delete(zoneIdToDelete), Times.Once);
            //_unitOfWorkMock.Verify(uow => uow.RepositoryZone.Commit(), Times.Once);
        }





    }
}
