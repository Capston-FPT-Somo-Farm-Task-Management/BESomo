using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.Zone;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var zones = await _zoneService.ListZone();
                return Ok(new ApiResponseModel
                {
                    Data = zones,
                    Message = "Zone is found",
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
        public async Task<IActionResult> ListZoneActive()
        {
            try
            {
                var zones = await _zoneService.ListActiveZone();
                return Ok(new ApiResponseModel
                {
                    Data = zones,
                    Message = "Zone is found",
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound("Zone is not found");
                }
                var zone = await _zoneService.GetZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = zone,
                    Message = "Zone is found",
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

        [HttpGet("Active/Area({id})")]
        public async Task<IActionResult> GetByArea(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetByArea(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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
        [HttpGet("Area({id})")]
        public async Task<IActionResult> GetAllByArea(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetAllByArea(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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
        [HttpGet("Farm({id})")]
        public async Task<IActionResult> GetByFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetByFarmId(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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
        [HttpGet("Active/Farm({id})")]
        public async Task<IActionResult> GetActiveByFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetActiveByFarmId(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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

        [HttpGet("ZoneType({id})")]
        public async Task<IActionResult> GetByZoneType(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetByZoneTypeId(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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

        [HttpGet("AreaPlant({id})")]
        public async Task<IActionResult> GetByAreaAndPlant(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetByAreaAndPlant(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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

        [HttpGet("AreaLivestock({id})")]
        public async Task<IActionResult> GetByAreaAndLivestock(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id must be greater than 0");
                }
                var area = await _zoneService.GetByAreaAndLivestock(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
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

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] ZoneCreateUpdateModel zone)
        {
            try
            {
                await _zoneService.AddZone(zone);
                var responseData = new ApiResponseModel
                {
                    Data = zone,
                    Message = "Thêm vùng thành công",
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ZoneCreateUpdateModel zone)
        {
            try
            {
                await _zoneService.UpdateZone(id, zone);
                var responseData = new ApiResponseModel
                {
                    Data = zone,
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _zoneService.Delete(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Delete success",
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

        [HttpPut("Delete/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _zoneService.UpdateStatus(id);
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
