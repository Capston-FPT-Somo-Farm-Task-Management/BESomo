using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
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
                return Ok(await _zoneService.ListZone());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
                var area = await _zoneService.GetZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Zone is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Area({id})")]
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
                return BadRequest(e.Message);

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
                return BadRequest(e.Message);

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
                return BadRequest(e.Message);
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
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] Zone zone)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _zoneService.AddZone(zone);
                    var responseData = new ApiResponseModel
                    {
                        Data = zone,
                        Message = "Zone is added",
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

                    response.Message = "Invalid zone data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Zone zone)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _zoneService.GetZone(id);
                    if (existingArea == null)
                    {
                        response.Message = "Zone not found";
                        return NotFound(response);
                    }
                    await _zoneService.UpdateZone(zone);
                    var responseData = new ApiResponseModel
                    {
                        Data = zone,
                        Message = "Zone is updated",
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

                    response.Message = "Invalid Zone data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = new ApiResponseModel();
                var existingArea = await _zoneService.GetZone(id);
                if (existingArea == null)
                {
                    response.Message = "Zone not found";
                    return NotFound(response);
                }

                await _zoneService.DeleteZone(existingArea);
                response.Message = "Zone is deleted";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
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
                    return NotFound("Zone id must be greater than 0 ");
                }
                await _zoneService.UpdateStatus(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Status livestock is updated",
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

                response.Message = "Invalid liveStock data: " + string.Join(" ", errorMessages);
                return BadRequest(response);
            }
        }
    }
}
