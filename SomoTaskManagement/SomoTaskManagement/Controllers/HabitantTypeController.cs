using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
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
        [HttpGet("PlantType/Farm({farmId})")]
        public async Task<IActionResult> ListPlantType(int farmId)
        {
            try
            {
                var area = await _hanbitantTypeService.ListPlantType(farmId);
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

        [HttpGet("PlantType/Active/Farm({farmId})")]
        public async Task<IActionResult> ListPlantTypeActive(int farmId)
        {
            try
            {
                var area = await _hanbitantTypeService.ListPlantTypeActive(farmId);
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

        [HttpGet("Active/Farm({farmId})")]
        public async Task<IActionResult> ListHabitantTypeActive(int farmId)
        {
            try
            {
                var area = await _hanbitantTypeService.ListHabitantTypeActive(farmId);
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

        [HttpGet("LivestockType/Farm({farmId})")]
        public async Task<IActionResult> ListLiveStock(int farmId)
        {
            try
            {
                var area = await _hanbitantTypeService.ListLiveStock(farmId);
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

        [HttpGet("LivestockType/Active/Farm({farmId})")]
        public async Task<IActionResult> ListLiveStockActive(int farmId)
        {
            try
            {
                var area = await _hanbitantTypeService.ListLiveStockActive(farmId);
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
        public async Task<IActionResult> CreateArea([FromBody] HabitantTypeCUModel habitant)
        {
            try
            {
                await _hanbitantTypeService.AddHabitantType(habitant);
                var responseData = new ApiResponseModel
                {
                    Data = habitant,
                    Message = "HabitantType is added",
                    Success = true,
                };
                return Ok(new ApiResponseModel
                {
                    Data = habitant,
                    Message = "Habitant is found",
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

                    await _hanbitantTypeService.UpdateStatus(id);
                    var responseData = new ApiResponseModel
                    {
                        Data = null,
                        Message = "Status is updated",
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

                    response.Message = "Invalid  data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HabitantTypeCUModel habitant)
        {
            try
            {
                await _hanbitantTypeService.UpdateHabitantType(id, habitant);
                var responseData = new ApiResponseModel
                {
                    Data = habitant,
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        var response = new ApiResponseModel();
        //        var existingArea = await _hanbitantTypeService.GetHabitant(id);
        //        if (existingArea == null)
        //        {
        //            response.Message = "Zone not found";
        //            return NotFound(response);
        //        }

        //        await _hanbitantTypeService.DeleteHabitantType(existingArea);
        //        response.Message = "Zone is deleted";
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
