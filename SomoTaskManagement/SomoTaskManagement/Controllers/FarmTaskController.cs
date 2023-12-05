using AutoMapper.Execution;
using DemoRedis.Attributes;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using System.Threading.Tasks;
using Notification = SomoTaskManagement.Domain.Entities.Notification;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Manager,Admin,Supervisor")]
    public class FarmTaskController : ControllerBase
    {
        private readonly IFarmTaskService _farmTaskService;
        private readonly IHubConnection _hubConnection;
        private readonly IUnitOfWork _unitOfWork;

        public FarmTaskController(IFarmTaskService farmTaskService, IHubConnection hubConnection, IUnitOfWork unitOfWork)
        {
            _farmTaskService = farmTaskService;
            _hubConnection = hubConnection;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetList();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("GetTotalTaskOfWeekByMember({memberId})")]
        public async Task<IActionResult> GetTotalTaskOfWeekByMember(int memberId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTotalTaskOfWeekByMember(memberId);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("GetTaskPrepareAndDoing/Member({id})")]
        public async Task<IActionResult> GetTaskPrepareAndDoing(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTaskPrepareAndDoing(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("GetTotalTaskOfFarm/Farm({id})")]
        public async Task<IActionResult> GetTotalTaskOfFarm(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTotalTaskOfFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }



        [HttpGet("GetTotalTaskOfFarmIncurrentMonth/Farm({id})")]
        public async Task<IActionResult> GetTotalTaskOfFarmIncurrentMonth(int id, int month)
        {
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTotalTaskOfFarmIncurrentMonth(id, month);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }


        [HttpGet("TaskActive/Page({pageIndex})/PageSize({pageSize})")]
        public async Task<IActionResult> GetListActiveWithPagging(int pageIndex, int pageSize)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetListActiveWithPagging(pageIndex, pageSize);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({id})/Disagree")]
        public async Task<IActionResult> CreateDisagreeTask(int id, [FromBody] string description)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.CreateDisagreeTask(id, description);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }
        [HttpGet("TaskActive")]
        public async Task<IActionResult> GetListActive()
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetListActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/Done/Employee({employeeId})")]
        public async Task<IActionResult> GetTaskByEmployeeDates(int employeeId, DateTime? startDay, DateTime? endDay, int pageIndex, int pageSize,int? status)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTaskByEmployeeDates(employeeId, startDay, endDay, pageIndex, pageSize, status);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.Get(id);
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("Member({id})/TaskDate/{date}")]
        public async Task<IActionResult> GetTaskByTotalDay(DateTime date, int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var task = await _farmTaskService.GetTaskByTotalDay(date, id);
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("Day/{date}")]
        public async Task<IActionResult> GetTaskByDay(DateTime date)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {


                var task = await _farmTaskService.GetTaskByDay(date);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("Member/{memberId}")]
        public async Task<IActionResult> GetTaskByMemberId(int memberId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.GetTaskByMemberId(memberId);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("Task({id})/Refuse")]
        public async Task<IActionResult> DisDisagreeTask(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.DisDisagreeTask(id);

                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({id})/ChangeStatusToClose")]
        public async Task<IActionResult> ChangeStatusToClose(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.ChangeStatusToClose(id);

                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({id})/ChangeStatusToDone")]
        public async Task<IActionResult> ChangeStatusToDone(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.ChangeStatusToDone(id);

                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPost("({id})/CreateTaskClone")]
        public async Task<IActionResult> CreateTaskClone(int id)
        {
            if (!User.IsInRole("Manager") )
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.CreateTaskClone(id);

                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Tạo thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/Manager({id})/Status({status})/Date")]
        public async Task<IActionResult> GetTaskByStatusMemberDate(int id, int status, [FromQuery] DateTime? date, int pageIndex, int pageSize, [FromQuery] int? checkTaskParent, [FromQuery] string? taskName)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.GetTaskByStatusMemberDate(id, status, date, pageIndex, pageSize, checkTaskParent, taskName);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });

            }
        }

        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/Manager({id})/Date")]
        public async Task<IActionResult> GetAllTaskByMemberDate(int id, [FromQuery] DateTime? date, int pageIndex, int pageSize, [FromQuery] int? checkTaskParent, [FromQuery] string? taskName)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.GetAllTaskByMemberDate(id, date, pageIndex, pageSize, checkTaskParent, taskName);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });

            }
        }

        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/Supervisor({id})/Status({status})/Date")]
        public async Task<IActionResult> GetTaskByStatusSupervisorDate(int id, int status, [FromQuery] DateTime? date, int pageIndex, int pageSize, [FromQuery] string? taskName)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.GetTaskByStatusSupervisorDate(id, status, date, pageIndex, pageSize, taskName);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách nhiệm vụ ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetStatusDescriptions()
        {

            try
            {
                var task = _farmTaskService.GetStatusDescriptions();

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Danh sách trạng thái ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("TaskActive/Member/{memberId}")]
        public async Task<IActionResult> GetListActiveByMemberId(int memberId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                var task = await _farmTaskService.GetListActiveByMemberId(memberId);

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Tìm thấy danh sách",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }
        [HttpGet("GetManagerId")]
        public async Task<IActionResult> GetManagerId()
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                List<string> deviceTokens = new List<string>();

                var listManager = await _hubConnection.GetManagerId();

                foreach (var manager in listManager)
                {
                    var managerTokens = await _hubConnection.GetTokenByMemberId(manager);

                    deviceTokens.AddRange(managerTokens);
                }


                return Ok(new ApiResponseModel
                {
                    Data = deviceTokens,
                    Message = "Tìm thấy danh sách",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPost("supervisor/CreateAsignTask")]
        public async Task<IActionResult> CreateAsignTask(CreateAsignTaskRequest createAsignTaskRequest)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.CreateAsignTask(createAsignTaskRequest.TaskModel, createAsignTaskRequest.MaterialIds, createAsignTaskRequest.EmployeeIds);

                return Ok(new ApiResponseModel
                {
                    Data = createAsignTaskRequest,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPost("CreateTaskToDo")]
        public async Task<IActionResult> CreateTaskToDo(TaskTodoRequestModel taskToDoModel)
        {
            if (!User.IsInRole("Manager") )
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.CreateTaskToDo(taskToDoModel.FarmTask, taskToDoModel.Dates,taskToDoModel.MaterialIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskToDoModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({taskId})/UpdateTaskDraftAndToPrePare")]
        public async Task<IActionResult> UpdateTaskDraftAndToPrePare(int taskId, UpdateTaskDraftAndToPrePare taskToDoModel)
        {
            if (!User.IsInRole("Manager"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.UpdateTaskDraftAndToPrePare(taskId,taskToDoModel.TaskModel, taskToDoModel.Dates, taskToDoModel.MaterialIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskToDoModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }


        [HttpPut("({taskId})/UpdateTaskAsign")]
        public async Task<IActionResult> UpdateTaskAsign(int taskId, UpdateTaskAsignRequest taskToDoModel)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.UpdateTaskAsign(taskId, taskToDoModel.TaskModel, taskToDoModel.MaterialIds, taskToDoModel.EmployeeIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskToDoModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpDelete("({taskId})/DeleteTaskAssign")]
        public async Task<IActionResult> DeleteTaskAssign(int taskId)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.DeleteTaskAssign(taskId );

                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Xóa thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({taskId})/UpdateTaskDisagreeAndChangeToToDo")]
        public async Task<IActionResult> UpdateTaskDisagreeAndChangeToToDo(int taskId, UpdateTaskDraftAndToPrePare taskToDoModel)
        {
            if (!User.IsInRole("Manager"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.UpdateTaskDisagreeAndChangeToToDo(taskId, taskToDoModel.TaskModel, taskToDoModel.Dates, taskToDoModel.MaterialIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskToDoModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPost("CreateTaskDraft")]
        public async Task<IActionResult> CreateTaskDraft(TaskDraftRequest taskDraftModel)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.CreateTaskDraft(taskDraftModel.FarmTask, taskDraftModel.Dates,taskDraftModel.MaterialIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskDraftModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("({taskId})/UpdateTask")]
        public async Task<IActionResult> UpdateTask(int taskId, RequestUpdateTaskDraft taskModel)
        {
            if (!User.IsInRole("Manager"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.UpdateTask(taskId,taskModel.TaskModel,taskModel.Dates,taskModel.MaterialIds);

                return Ok(new ApiResponseModel
                {
                    Data = taskModel,
                    Message = "Cập nhật thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        

        [HttpPut("({id})/UpdateStatusFromTodoToDraft")]
        public async Task<IActionResult> UpdateStatusFormTodoToDraft(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                //var task = await _farmTaskService.Get(id);
                await _farmTaskService.UpdateStatusFormTodoToDraft(id);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được cập nhật",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }

        [HttpPut("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") )
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.DeleteTask(id);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được xóa",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }

        [HttpDelete("({id})")]
        public async Task<IActionResult> DeleteTaskTodoDraftAssign(int id)
        {
            if (!User.IsInRole("Manager"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.DeleteTaskTodoDraftAssign(id);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được xóa",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }



        [HttpPut("({id})/AddEmployeeToTaskAsign")]
        public async Task<IActionResult> AddEmployeeToTaskAsign(int id, AddEmployeeToTaskAsign employeeToTaskAsign)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.AddEmployeeToTaskAsign(id, employeeToTaskAsign.EmployeeIds,employeeToTaskAsign.OverallEfforMinutes,employeeToTaskAsign.OverallEffortHour);

                var responseData = new ApiResponseModel
                {
                    Data = employeeToTaskAsign,
                    Message = "Nhiệm vụ đã được cập nhật",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }

        [HttpPut("({id})/ChangeStatusToDoing")]
        public async Task<IActionResult> ChangeStatusToDoing(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.ChangeStatusToDoing(id);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được cập nhật",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }

        [HttpPut("({id})/ChangeStatusToPendingAndCancel")]
        public async Task<IActionResult> ChangeStatusToPendingAndCancel(int id, [FromForm] EvidencePendingAndCancel taskEvidence, int status,int? managerId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.ChangeStatusToPendingAndCancel(id, taskEvidence,status, managerId);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được cập nhật",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }

        [HttpPut("({id})/ChangeStatusFromDoneToDoing")]
        public async Task<IActionResult> ChangeStatusFromDoneToDoing(int id,[FromBody] string reason, int managerId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _farmTaskService.ChangeStatusFromDoneToDoing(id, reason, managerId);

                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã được cập nhật",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }

        }
    }
}
