using Moq;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoFarmTaskManagement.Test
{
    public class MaterialTest
    {
        private Mock<IMaterialService> _materialServiceMock;
        private Mock<IFarmService> _farmServiceMock;

        [SetUp]
        public void SetUp()
        {
            _materialServiceMock = new Mock<IMaterialService>();
            _farmServiceMock = new Mock<IFarmService>();
        }

        //Test list material success
        [Test]
        public async Task ListMaterial_Should_Return_MaterialModel()
        {
            _materialServiceMock.Setup(materialService => materialService.ListMaterial())
                    .ReturnsAsync(new List<MaterialModel> { new MaterialModel() });

            var materials = await _materialServiceMock.Object.ListMaterial();

            Assert.IsNotNull(materials, "Danh sách areas không được null");
            Assert.IsInstanceOf<IEnumerable<MaterialModel>>(materials, "Danh sách areas phải là một IEnumerable<AreaModel>");
            Assert.IsTrue(materials.Any(), "Danh sách areas phải chứa ít nhất một phần tử");
        }


        //Test all material by farmId success
        [Test]
        public async Task GetMaterialByFarmId_All_Should_Return_ListMaterialModel()
        {
            _materialServiceMock.Setup(materialService => materialService.ListMaterialByFarm(It.IsAny<int>())).
                    ReturnsAsync((int farmId) => new List<MaterialModel> { new MaterialModel() { } });

            int farmId = 1;

            var materials = await _materialServiceMock.Object.ListMaterialByFarm(farmId);

            Assert.IsNotNull(materials, "Danh sách materials không được null");
            Assert.IsInstanceOf<IEnumerable<MaterialModel>>(materials, "Danh sách materials phải là một IEnumerable<MaterialModel>");
        }

        //Test list all material by farmId fail(Error can not find farmId)
        [Test]
        public async Task GetMaterialByFarmId_Should_Return_Exception()
        {
            int farmId = 1;

            _farmServiceMock.Setup(farmServiceMock => farmServiceMock.GetFarmById(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy trang trại"));
            Exception exception = null;
            try
            {
                var farm = await _farmServiceMock.Object.GetFarmById(farmId);
                await _materialServiceMock.Object.ListMaterialByFarm(farm.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy trang trại", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }

        //Test active material by farmId success
        [Test]
        public async Task GetMaterialByFarmId_Active_Should_Return_ListMaterialModel()
        {
            _materialServiceMock.Setup(materialService => materialService.ListMaterialActive(It.IsAny<int>())).
                    ReturnsAsync((int farmId) => new List<MaterialModel> { new MaterialModel() { } });

            int farmId = 1;

            var materials = await _materialServiceMock.Object.ListMaterialByFarm(farmId);

            Assert.IsNotNull(materials, "Danh sách materials không được null");
            Assert.IsInstanceOf<IEnumerable<MaterialModel>>(materials, "Danh sách materials phải là một IEnumerable<MaterialModel>");
        }

        //Test list active material by farmId fail(Error can not find farmId)
        [Test]
        public async Task GetMaterialByFarmId_Active_Should_Return_Exception()
        {
            int farmId = 1;

            _farmServiceMock.Setup(farmServiceMock => farmServiceMock.GetFarmById(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Không tìm thấy trang trại"));
            Exception exception = null;
            try
            {
                var farm = await _farmServiceMock.Object.GetFarmById(farmId);
                await _materialServiceMock.Object.ListMaterialActive(farm.Id);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                exception = ex;
                Assert.AreEqual("Không tìm thấy trang trại", ex.Message);
            }

            Assert.IsNotNull(exception, "Exception should not be null");
        }


        //Test get material by id cuccess
        [Test]
        public async Task GetFarmById_Should_Return_Material()
        {
            _materialServiceMock.Setup(materialService => materialService.GetMaterial(It.IsAny<int>()))
             .ReturnsAsync((int materialId) => new Material { Id = materialId, Name = "Material test" });

            int materialId = 1;
            var expectedMaterial = new Material { Id = materialId, Name = "Material test" };

            var material = await _materialServiceMock.Object.GetMaterial(materialId);

            Assert.IsNotNull(material, "Area không được null");
            Assert.AreEqual(expectedMaterial.Id, material.Id, "Id của Area không khớp");
        }

        [Test]
        public async Task GetFarmById_Should_Return_Exception()
        {
            _materialServiceMock.Setup(materialService => materialService.GetMaterial(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Dụng cụ không tìm thấy"));

            int materialId = 2;
            try
            {
                await _materialServiceMock.Object.GetMaterial(materialId);

                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Dụng cụ không tìm thấy", ex.Message);
            }
        }



    }
}