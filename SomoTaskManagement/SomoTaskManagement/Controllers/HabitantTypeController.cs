using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitantTypeController : ControllerBase
    {
        private readonly IHanbitantTypeService _hanbitantTypeService;

        public HabitantTypeController(IHanbitantTypeService hanbitantTypeService)
        {
            _hanbitantTypeService = hanbitantTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _hanbitantTypeService.ListHabitantType());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("PlantType")]
        public async Task<IActionResult> ListPlantType()
        {
            try
            {
                var area = await _hanbitantTypeService.ListPlantType();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "List plant type success",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("LivestockType")]
        public async Task<IActionResult> ListLiveStock()
        {
            try
            {
                var area = await _hanbitantTypeService.ListLiveStock();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "List livestokc type success",
                    Success = true,
                });
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
                    return NotFound("HabitantType is not found");
                }
                var area = await _hanbitantTypeService.GetHabitant(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "HabitantType is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] HabitantType habitant)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _hanbitantTypeService.AddHabitantType(habitant);
                    var responseData = new ApiResponseModel
                    {
                        Data = habitant,
                        Message = "HabitantType is added",
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

                    response.Message = "Invalid HabitantType data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HabitantType habitant)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _hanbitantTypeService.GetHabitant(id);
                    if (existingArea == null)
                    {
                        response.Message = "HabitantType not found";
                        return NotFound(response);
                    }
                    await _hanbitantTypeService.UpdateHabitantType(habitant);
                    var responseData = new ApiResponseModel
                    {
                        Data = habitant,
                        Message = "HabitantType is updated",
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
                var existingArea = await _hanbitantTypeService.GetHabitant(id);
                if (existingArea == null)
                {
                    response.Message = "Zone not found";
                    return NotFound(response);
                }

                await _hanbitantTypeService.DeleteHabitantType(existingArea);
                response.Message = "Zone is deleted";
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
