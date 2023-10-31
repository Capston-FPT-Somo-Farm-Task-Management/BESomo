using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmSubTaskController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;

        public FarmSubTaskController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        [HttpGet("Task({taskId})")]
        public async Task<IActionResult> SubtaskByTask(int taskId)
        {
            try
            {
                var subtask = await _subTaskService.SubtaskByTask(taskId);
                return Ok(new ApiResponseModel
                {
                    Data = subtask,
                    Message = "Nhiệm vụ đã tìm thấy",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }

        [HttpGet("Task({taskId})/Effort")]
        public async Task<IActionResult> GetEffortByTask(int taskId)
        {
            try
            {
                var subtask = await _subTaskService.GetEffortByTask(taskId);
                return Ok(new ApiResponseModel
                {
                    Data = subtask,
                    Message = "Nhiệm vụ đã tìm thấy",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }

        [HttpGet("Employee({memberId})/TotalEffort")]
        public async Task<IActionResult> GetTotalEffortEmployee(int memberId, [FromQuery]DateTime? startDay, [FromQuery]DateTime? endDay)
        {
            try
            {
                var subtask = await _subTaskService.GetTotalEffortEmployee(memberId,startDay,endDay);
                return Ok(new ApiResponseModel
                {
                    Data = subtask,
                    Message = "Nhiệm vụ đã tìm thấy",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }

        [HttpGet("EmployeeNoSubtask({taskId})")]
        public async Task<IActionResult> GetEmployeesNoSubtask(int taskId)
        {
            try
            {
                var subtask = await _subTaskService.GetEmployeesNoSubtask(taskId);
                return Ok(new ApiResponseModel
                {
                    Data = subtask,
                    Message = "Đã tìm thấy",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }

        [HttpPost("Task")]
        public async Task<IActionResult> CreateSubTasks(SubTaskCreateModel subTask)
        {
            try
            {
                await _subTaskService.CreateSubTasks(subTask);
                return Ok(new ApiResponseModel
                {
                    Data = subTask,
                    Message = "Nhiệm vụ đã cập nhật thành công",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }
        [HttpPut("Delete/Task({taskId})/Employee({employeeId})")]
        public async Task<IActionResult> DeleteSubTasks(int taskId, int employeeId)
        {
            try
            {
                await _subTaskService.DeleteSubTasks(taskId,employeeId);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã cập nhật thành công",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }


        [HttpPut("Task({taskId})")]
        public async Task<IActionResult> UpdateEffortTime(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes)
        {
            try
            {
                await _subTaskService.UpdateEffortTime(taskId,employeeEffortTimes);
                return Ok(new ApiResponseModel
                {
                    Data = employeeEffortTimes,
                    Message = "Nhiệm vụ đã cập nhật thành công",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }

        [HttpPut("Task({taskId})/Status({statusTask})")]
        public async Task<IActionResult> UpdateEffortTimeAndStatusTask(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes, int statusTask)
        {
            try
            {
                await _subTaskService.UpdateEffortTimeAndStatusTask(taskId, employeeEffortTimes,statusTask);
                return Ok(new ApiResponseModel
                {
                    Data = employeeEffortTimes,
                    Message = "Nhiệm vụ đã cập nhật thành công",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = ex.Message,
                    Success = true
                });
            }
        }
    }
}
