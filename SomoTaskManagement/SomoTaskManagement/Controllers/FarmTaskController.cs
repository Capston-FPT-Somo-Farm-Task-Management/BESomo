using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmTaskController : ControllerBase
    {
        private readonly IFarmTaskService _farmTaskService;

        public FarmTaskController(IFarmTaskService farmTaskService)
        {
            _farmTaskService = farmTaskService;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var area = await _farmTaskService.GetList();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "List task success",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("TaskActive")]
        public async Task<IActionResult> GetListActive()
        {
            try
            {
                var area = await _farmTaskService.GetListActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "List task success",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Task id must be greater than 0");
                }
                var task = await _farmTaskService.Get(id);
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Task is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Day/{date}")]
        public async Task<IActionResult> GetTaskByDay(DateTime date)
        {
            try
            {
                if (date == null)
                {
                    return NotFound("Task is not found");
                }

                var task = await _farmTaskService.GetTaskByDay(date);
                if (task == null)
                {
                    return BadRequest("Not found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Task is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Member/{memberId}")]
        public async Task<IActionResult> GetTaskByMemberId(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    return NotFound("Member id must be greater than 0 ");
                }
                var task = await _farmTaskService.GetTaskByMemberId(memberId);
                if (task == null)
                {
                    return NotFound("Task not found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Task is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("TaskActive/Member/{memberId}")]
        public async Task<IActionResult> GetListActiveByMemberId(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    return NotFound("Member id must be greater than 0 ");
                }
                var task = await _farmTaskService.GetListActiveByMemberId(memberId);
                if (task == null)
                {
                    return NotFound("Task not found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "List task Active By MemberId success",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(int memberId, [FromBody] TaskRequestModel taskModel)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _farmTaskService.Add(memberId, taskModel.FarmTask, taskModel.EmployeeIds, taskModel.MaterialIds);
                    var responseData = new ApiResponseModel
                    {
                        Data = taskModel,
                        Message = "Task is added",
                        Success = true,
                    };
                    return Ok(responseData);
                }
                else
                {
                    var errorMessages = new List<string>();
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errorMessages.Add(modelError.ErrorMessage);
                    }

                    response.Message = "Invalid Habitant data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpPut("Task({taskId})/Member({memberId})")]
        //public async Task<IActionResult> UpdateArea(int taskId, int memberId, [FromBody] TaskRequestModel taskModel)
        //{
        //    if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
        //    {
        //        return Unauthorized("You do not have access to this method.");
        //    }
        //    var response = new ApiResponseModel();
        //    if (ModelState.IsValid)
        //    {
        //        var existingArea = await _farmTaskService.Get(taskId);
        //        if (existingArea == null)
        //        {
        //            response.Message = "FarmTask not found";
        //            return NotFound(response);
        //        }
        //        await _farmTaskService.Update(taskId, memberId, taskModel.FarmTask, taskModel.EmployeeIds, taskModel.MaterialIds);
        //        var responseData = new ApiResponseModel
        //        {
        //            Data = taskModel,
        //            Message = "FarmTask is updated",
        //            Success = true,
        //        };
        //        return Ok(responseData);
        //    }
        //    else
        //    {
        //        var errorMessages = new List<string>();
        //        foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            errorMessages.Add(modelError.ErrorMessage);
        //        }

        //        response.Message = "Invalid FarmTask data: " + string.Join(" ", errorMessages);
        //        return BadRequest(response);
        //    }
        //}

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            var response = new ApiResponseModel();
            if (ModelState.IsValid)
            {
                if (id <= 0)
                {
                    return NotFound("Task id must be greater than 0 ");
                }
                await _farmTaskService.UpdateStatus(id, status);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "FarmTask is updated",
                    Success = true,
                };
                return Ok(responseData);
            }
            else
            {
                var errorMessages = new List<string>();
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    errorMessages.Add(modelError.ErrorMessage);
                }

                response.Message = "Invalid FarmTask data: " + string.Join(" ", errorMessages);
                return BadRequest(response);
            }
        }
    }
}
