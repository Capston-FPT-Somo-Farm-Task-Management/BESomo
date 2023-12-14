using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Manager,Supervisor")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;

        public ActivitiesController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        [HttpGet("Task({taskId})")]
        public async Task<IActionResult> SubtaskByTask(int taskId,[FromQuery]int? employeeId)
        {
            try
            {
                var subtask = await _subTaskService.SubtaskByTask(taskId, employeeId);
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

        [HttpGet("Task({taskId})/NonSubtaskByTask")]
        public async Task<IActionResult> NonSubtaskByTask(int taskId)
        {
            try
            {
                var subtask = await _subTaskService.NonSubtaskByTask(taskId);
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
        public async Task<IActionResult> GetTotalEffortEmployee(int memberId, [FromQuery] DateTime? startDay, [FromQuery] DateTime? endDay)
        {
            try
            {
                var subtask = await _subTaskService.GetTotalEffortEmployee(memberId, startDay, endDay);
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

        [HttpGet("EmployeeNoActivities({taskId})")]
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

        [HttpPost]
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

        [HttpPut("({subtaskId})")]
        public async Task<IActionResult> UpdateSubTasks(int subtaskId, SubTaskUpdateModel subTask)
        {
            try
            {
                await _subTaskService.UpdateSubTasks(subtaskId,subTask);
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

        [HttpDelete("Delete({subtaskId})")]
        public async Task<IActionResult> DeleteSubTasks(int subtaskId)
        {
            try
            {
                await _subTaskService.DeleteSubTasks(subtaskId);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Nhiệm vụ đã xoá thành công",
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

        [HttpPut("({subtaskId})/Effort")]
        public async Task<IActionResult> UpdateEffortOfSubtask(int subtaskId, EmployeeEffortUpdate employeeEffortTime)
        {
            try
            {
                await _subTaskService.UpdateEffortOfSubtask(subtaskId, employeeEffortTime);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Chấm công thành công",
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
                await _subTaskService.UpdateEffortTime(taskId, employeeEffortTimes);
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

        [HttpPut("Task({taskId})/UpdateEffortTimeAndStatusTask")]
        public async Task<IActionResult> UpdateEffortTimeAndStatusTask(int taskId, List<EmployeeEffortUpdateAndChangeStatus> employeeEffortTimes)
        {
            try
            {
                await _subTaskService.UpdateEffortTimeAndStatusTask(taskId, employeeEffortTimes);
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
