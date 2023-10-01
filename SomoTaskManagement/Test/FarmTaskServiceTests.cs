//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SomoTaskManagement.Domain.Model;
//using SomoTaskManagement.Data.Abtract;
//using SomoTaskManagement.Domain.Entities;
//using SomoTaskManagement.Services.Imp;
//using SomoTaskManagement.Services.Interface;
//using AutoMapper;
//using Moq;
//using Xunit;

//public class FarmTaskServiceTests
//{
//    private IFarmTaskService _farmTaskService;
//    private Mock<IUnitOfWork> _unitOfWorkMock;
//    private Mock<IMapper> _mapperMock;
//    public FarmTaskServiceTests()
//    {
//        _unitOfWorkMock = new Mock<IUnitOfWork>();
//        _mapperMock = new Mock<IMapper>();

//        // Khởi tạo với cả 2 tham số 
//        _farmTaskService = new FarmTaskService(_unitOfWorkMock.Object, _mapperMock.Object);
//    }

//    [Fact]
//    public async Task Add_DailyRepeat_ShouldCreateRepeatedTasks()
//    {
//        // Arrange
//        var memberId = 1;
//        var farmTaskModel = new TaskCreateUpdateModel
//        {
//            StartDate = DateTime.Today,
//            EndDate = DateTime.Today.AddDays(5),
//            Repeat = "Hằng ngày",
//            Iterations = 2,
//            Name = "Chan heo",
//            Description = "heo",
//            Priority = 1,
//            ReceiverId = 1,
//            FieldId = 1,
//            TaskTypeId = 3,
//            MemberId = 1,
//            OtherId = 1,
//            PlantId = 1,
//            LiveStockId = 2,
//            Remind = 1,
//        };

//        var employeeIds = new List<int>(1);
//        var materialIds = new List<int>(1);

//        // Act
//        await _farmTaskService.Add(memberId, farmTaskModel, employeeIds, materialIds);

//        // Assert
//        _unitOfWorkMock.Setup(x => x.RepositoryMember.GetById(It.IsAny<int>)).ReturnsAsync(new Member());

//        _unitOfWorkMock.Setup(x => x.RepositoryEmployee.GetById(It.IsAny<int>)).ReturnsAsync(new Employee());

//        _unitOfWorkMock.Verify(x => x.RepositoryFarmTask.Add(
//            It.Is<FarmTask>(t => t.StartDate == DateTime.Today)), Times.Exactly(2));

//        _unitOfWorkMock.Verify(x => x.RepositoryFarmTask.Add(
//            It.Is<FarmTask>(t => t.StartDate == DateTime.Today.AddDays(1))), Times.Exactly(2));
//    }
//}
