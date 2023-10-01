using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneTypeController : ControllerBase
    {
        private readonly IZoneTypeService _zoneTypeService;

        public ZoneTypeController(IZoneTypeService zoneTypeService)
        {
            _zoneTypeService = zoneTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _zoneTypeService.ListZone());
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
                    return NotFound("ZoneType is not found");
                }
                var area = await _zoneTypeService.GetZoneType(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "ZoneType is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] ZoneType zoneType)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _zoneTypeService.AddZoneType(zoneType);
                    var responseData = new ApiResponseModel
                    {
                        Data = zoneType,
                        Message = "ZoneType is added",
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

                    response.Message = "Invalid ZoneType data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ZoneType zoneType)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _zoneTypeService.GetZoneType(id);
                    if (existingArea == null)
                    {
                        response.Message = "ZoneType not found";
                        return NotFound(response);
                    }
                    await _zoneTypeService.UpdateZoneType(zoneType);
                    var responseData = new ApiResponseModel
                    {
                        Data = zoneType,
                        Message = "ZoneType is updated",
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

                    response.Message = "Invalid ZoneType data: " + string.Join(" ", errorMessages);
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
                var existingArea = await _zoneTypeService.GetZoneType(id);
                if (existingArea == null)
                {
                    response.Message = "ZoneType not found";
                    return NotFound(response);
                }

                await _zoneTypeService.DeleteZoneType(existingArea);
                response.Message = "ZoneType is deleted";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
