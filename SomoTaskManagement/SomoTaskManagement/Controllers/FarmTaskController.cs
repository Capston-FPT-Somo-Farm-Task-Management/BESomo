﻿using AutoMapper.Execution;
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
using SomoTaskManagement.Notify.FirebaseNotify;
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
        private readonly FcmNotificationService _fcmService;
        private readonly IHubConnection _hubConnection;
        private readonly IUnitOfWork _unitOfWork;

        public FarmTaskController(IFarmTaskService farmTaskService, IHubConnection hubConnection, IUnitOfWork unitOfWork)
        {
            _farmTaskService = farmTaskService;
            _fcmService = new FcmNotificationService();
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
        [HttpGet("GetTotalTypeOfTaskInWeekByMember({memberId})")]
        public async Task<IActionResult> GetTotalTypeOfTaskInWeekByMember(int memberId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _farmTaskService.GetTotalTypeOfTaskInWeekByMember(memberId);
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
        public async Task<IActionResult> GetTaskByEmployeeDates(int employeeId, DateTime? startDay, DateTime? endDay, int pageIndex, int pageSize, [FromQuery] int? status)
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
            if (!User.IsInRole("Manager"))
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

        [HttpPost]
        public async Task<IActionResult> ProcessTaskCreation(int memberId, [FromBody] TaskRequestModel taskModel)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                await _farmTaskService.ProcessTaskCreation(memberId, taskModel);

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

        [HttpPut("{taskId}")]
        public async Task<IActionResult> Update(int taskId, [FromBody] TaskRequestModel taskModel)
        {
            if (!User.IsInRole("Manager"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }

            try
            {
                await _farmTaskService.Update(taskId, taskModel);
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

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                //var task = await _farmTaskService.Get(id);
                await _farmTaskService.UpdateStatus(id, status);

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
    }
}
