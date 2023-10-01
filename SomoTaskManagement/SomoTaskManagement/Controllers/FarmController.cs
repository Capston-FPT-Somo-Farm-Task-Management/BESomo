using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmController : ControllerBase
    {
        private readonly IFarmService _farmService;

        public FarmController(IFarmService farmService)
        {
            _farmService = farmService;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _farmService.ListFarm());
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
                var area = await _farmService.GetFarmById(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Area is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] Farm farm)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {

                    await _farmService.CreateFarm(farm);
                    var responseData = new ApiResponseModel
                    {
                        Data = farm,
                        Message = "Farm is added",
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

                    response.Message = "Invalid Farm data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Farm farm)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _farmService.GetFarmById(id);
                    if (existingArea == null)
                    {
                        response.Message = "Farm not found";
                        return NotFound(response);
                    }
                    await _farmService.UpdateFarm(farm);
                    var responseData = new ApiResponseModel
                    {
                        Data = farm,
                        Message = "Farm is updated",
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

                    response.Message = "Invalid Farm data: " + string.Join(" ", errorMessages);
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
                var existingArea = await _farmService.GetFarmById(id);
                if (existingArea == null)
                {
                    response.Message = "Farm not found";
                    return NotFound(response);
                }

                await _farmService.DeleteFarm(existingArea);
                response.Message = "Farm is deleted";
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
