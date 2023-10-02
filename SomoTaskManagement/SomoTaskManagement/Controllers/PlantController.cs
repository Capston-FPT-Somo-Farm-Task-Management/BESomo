using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _plantService.GetList());
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
                    return NotFound("Habitant is not found");
                }
                var area = await _plantService.Get(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Habitant is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Farm({id})")]
        public async Task<IActionResult> GetPlantFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Plant id must be greater than 0");
                }
                var area = await _plantService.GetPlantFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Plant is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetListActive()
        {
            try
            {
                var area = await _plantService.GetListActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Plant is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlantCreateModel plant)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _plantService.Add(plant);
                    var responseData = new ApiResponseModel
                    {
                        Data = plant,
                        Message = "Plant is added",
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

                    response.Message = "Invalid Plant data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("ExternalId/Field({id})")]
        public async Task<IActionResult> GetExternalId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Field id must be greater than 0");
                }
                var area = await _plantService.GetExternalIds(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "External plant is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Plant plant)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _plantService.Get(id);
                    if (existingArea == null)
                    {
                        response.Message = "Habitant not found";
                        return NotFound(response);
                    }
                    await _plantService.Update(plant);
                    var responseData = new ApiResponseModel
                    {
                        Data = plant,
                        Message = "Plant is updated",
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

                    response.Message = "Invalid Plant data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    if (id <= 0)
                    {
                        return NotFound("Plant id must be greater than 0 ");
                    }
                    await _plantService.UpdateStatus(id);
                    var responseData = new ApiResponseModel
                    {
                        Data = null,
                        Message = "Status plant is updated",
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        var response = new ApiResponseModel();
        //        var existingArea = await _habitantService.GetHabitant(id);
        //        if (existingArea == null)
        //        {
        //            response.Message = "Habitant not found";
        //            return NotFound(response);
        //        }

        //        await _habitantService.DeleteHabitant(existingArea);
        //        response.Message = "Habitant is deleted";
        //        response.Success = true;
        //        return Ok(response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
    }
}
