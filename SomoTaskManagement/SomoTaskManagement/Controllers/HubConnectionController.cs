using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HubConnection;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubConnectionController : ControllerBase
    {
        private readonly IHubConnection _hubConnection;

        public HubConnectionController(IHubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        [HttpPost]
        public async Task<IActionResult> Create(HubConnection hubConnection)
        {
            try
            {
                await _hubConnection.Create(hubConnection);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Tạo thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTokenByMemberId(int id)
        {
            try
            {
                var connection = await _hubConnection.GetTokenByMemberId(id);
                return Ok(new ApiResponseModel
                {
                    Data = connection,
                    Message = "Tạo thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("GetManagerId")]
        public async Task<IActionResult> GetManagerId()
        {
            try
            {
                var connection = await _hubConnection.GetManagerId();
                return Ok(new ApiResponseModel
                {
                    Data = connection,
                    Message = "Tạo thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] HubConnectionDeleteModel token)
        {
            try
            {
                await _hubConnection.Delete(token);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Xóa thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
    }
}
