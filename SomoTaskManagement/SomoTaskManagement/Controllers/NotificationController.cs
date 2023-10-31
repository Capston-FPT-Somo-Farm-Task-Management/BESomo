using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("IsNew/Member{id}")]
        public async Task<IActionResult> GetList(int id)
        {
            try
            {
                var notifications = await _notificationService.ListByMemberIsNew(id);
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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
        [HttpGet("Read/Member{id}")]
        public async Task<IActionResult> ListByMemberRead(int id)
        {
            try
            {
                var notifications = await _notificationService.ListByMemberRead(id);
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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
        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/NotSeen/Member{id}")]
        public async Task<IActionResult> ListByMemberIsRead(int id, int pageIndex, int pageSize)
        {
            try
            {
                var notifications = await _notificationService.ListByMemberIsRead(id, pageIndex, pageSize);
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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
        [HttpGet("New/Member({id})/Count")]
        public async Task<IActionResult> GetCount(int id)
        {
            try
            {
                var notifications = await _notificationService.GetCount(id);
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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

        [HttpGet("PageIndex({pageIndex})/PageSize({pageSize})/Member({id})")]
        public async Task<IActionResult> ListByMember(int id, int pageIndex, int pageSize)
        {
            try
            {
                var notifications = await _notificationService.ListByMember(id,pageIndex,pageSize);
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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

        [HttpPut("IsNew/MemberId({id})")]
        public async Task<IActionResult> UpdateIsNew(int id)
        {
            try
            {
                await _notificationService.UpdateIsNew(id);
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


        [HttpPut("IsRead({id})")]
        public async Task<IActionResult> UpdateIsRead(int id)
        {
            try
            {
                await _notificationService.UpdateIsRead(id);
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
        [HttpPut("All/IsRead/Member({id})")]
        public async Task<IActionResult> UpdateAllIsRead(int id)
        {
            try
            {
                await _notificationService.UpdateAllIsRead(id);
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
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var notifications = await _notificationService.List();
                return Ok(new ApiResponseModel
                {
                    Data = notifications,
                    Message = "Tìm thấy danh sách ",
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
    }
}
