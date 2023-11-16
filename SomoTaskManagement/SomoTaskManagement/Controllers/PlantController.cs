using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Plant;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
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
                var area = await _plantService.GetList();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách",
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
                var area = await _plantService.Get(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách",
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

        [HttpGet("Active/Farm({id})")]
        public async Task<IActionResult> GetPlantActiveFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Livestock id must be greater than 0");
                }
                var area = await _plantService.GetPlantActiveFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách",
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
        public async Task<IActionResult> GetPlantFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Livestock id must be greater than 0");
                }
                var area = await _plantService.GetPlantFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách",
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

        [HttpGet("Active")]
        public async Task<IActionResult> GetListActive()
        {
            try
            {
                var area = await _plantService.GetListActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy danh sách",
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
                await _plantService.Add(plant);
                var responseData = new ApiResponseModel
                {
                    Data = plant,
                    Message = "Tìm thấy danh sách",
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
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlantCreateModel plant)
        {
            try
            {
                await _plantService.Update(id, plant);
                var responseData = new ApiResponseModel
                {
                    Data = plant,
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

        [HttpPut("Delete/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _plantService.UpdateStatus(id);
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
