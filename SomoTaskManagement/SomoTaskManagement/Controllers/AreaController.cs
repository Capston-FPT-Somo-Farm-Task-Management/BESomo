using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]

    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListArea()
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("Bạn không có quyền truy cập");
            //}
            try
            {
                var area = await _areaService.ListArea();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArea(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.GetArea(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }
            
        }

        [HttpGet("GetAreaWithZoneTypeLiveStock/Farm({farmId})")]
        public async Task<IActionResult> GetAreaWithZoneTypeLiveStock(int farmId)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.GetAreaWithZoneTypeLiveStock(farmId);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }

        }

        [HttpGet("GetAreaWithZoneTypePlant/Farm({farmId})")]
        public async Task<IActionResult> GetAreaWithZoneTypePlant(int farmId)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.GetAreaWithZoneTypePlant(farmId);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }

        }

        [HttpPut("Delete/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _areaService.DeleteByStatus(id);
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
                    Message = e.Message,
                    Success = false
                });
            }

        }
        [HttpGet("Active/Farm({id})")]
        public async Task<IActionResult> GetAreaByFarm(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.GetAreaByFarmId(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
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

        [HttpGet("Farm({id})")]
        public async Task<IActionResult> GetAllAreaByFarmId(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.GetAllAreaByFarmId(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = false,
                });
            }
        }
        [HttpGet("Active")]
        public async Task<IActionResult> GetAreaActive()
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var area = await _areaService.ListAreaActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaCreateUpdateModel area)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                var response = new ApiResponseModel();

                await _areaService.AddArea(area);
                var responseData = new ApiResponseModel
                {
                    Data = area,
                    Message = "Thêm thành công",
                    Success = true,
                };
                return Ok(responseData);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = e.Message,
                    Success = false
                });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaCreateUpdateModel area)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _areaService.UpdateArea(id, area);
                var responseData = new ApiResponseModel
                {
                    Data = area,
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
                    Success = false,
                });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _areaService.DeleteArea(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Xóa thành công",
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
                    Success = false,
                });
            }
        }
    }
}
