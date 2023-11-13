using Moq;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Field;
using SomoTaskManagement.Domain.Model.Zone;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _areaServiceMock = new Mock<IAreaService>();
            _farmServiceMock = new Mock<IFarmService>();
            _zoneServiceMock = new Mock<IZoneService>();
            _fieldServiceMock = new Mock<IFieldService>();
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


        //Delete area success
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

        //Delete zone fail (Error have the entity inside)
        //[Test]
        //public async Task DeleteZone_HaveTheEntityInside_ShouldThrowException()
        //{
        //    // Arrange
        //    _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.GetZone(It.IsAny<int>()))
        //    .ReturnsAsync((int zoneId) => new ZoneModel { Id = zoneId, Name = "Zone test" });

        //    _fieldServiceMock.Setup(fieldService => fieldService.ListFieldActive())
        //        .ReturnsAsync(new List<FieldModel> { new FieldModel() });

        //    _zoneServiceMock.Setup(zoneServiceMock => zoneServiceMock.Delete(It.IsAny<int>()))
        //            .ReturnsAsync((int zoneId) => Task.FromResult(new ZoneModel { Id = zoneId, Name = "Zone test" }));

        //    // Act & Assert
        //    Exception exception = null;
        //    try
        //    {
        //        await _zoneServiceMock.Object.Delete(1);
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex;
        //    }

        //    // Verify that DeleteArea was called
        //    _zoneServiceMock.Verify(a => a.Delete(1), Times.Once);

        //    // Assert
        //    Assert.IsNotNull(exception);
        //    Assert.AreEqual("Không thể xóa vùng này  khi còn thực thể bên trong", exception.Message);
        //}
    }
}
