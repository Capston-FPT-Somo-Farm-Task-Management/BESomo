using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Manager")]
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
            //    return Unauthorized("You do not have access to this method.");
            //}
            return Ok(await _areaService.ListArea());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArea(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            if (id == 0)
            {
                return NotFound("Area is not found");
            }
            var area = await _areaService.GetArea(id);
            return Ok(new ApiResponseModel
            {
                Data = area,
                Message = "Area is found",
                Success = true,
            });
        }

        [HttpGet("Farm({id})")]
        public async Task<IActionResult> GetAreaByFarm(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            if (id <= 0)
            {
                return NotFound("Farm id must be greater than 0");
            }
            var area = await _areaService.GetAreaByFarm(id);
            return Ok(new ApiResponseModel
            {
                Data = area,
                Message = "Area is found",
                Success = true,
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] Area area)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            var response = new ApiResponseModel();
            if (ModelState.IsValid)
            {
                await _areaService.AddArea(area);
                var responseData = new ApiResponseModel
                {
                    Data = area,
                    Message = "Area is added",
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] Area area)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            var response = new ApiResponseModel();
            if (ModelState.IsValid)
            {
                var existingArea = await _areaService.GetArea(id);
                if (existingArea == null)
                {
                    response.Message = "Area not found";
                    return NotFound(response);
                }
                await _areaService.UpdateArea(area);
                var responseData = new ApiResponseModel
                {
                    Data = area,
                    Message = "Area is updated",
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            var response = new ApiResponseModel();
            var existingArea = await _areaService.GetArea(id);
            if (existingArea == null)
            {
                response.Message = "Area not found";
                return NotFound(response);
            }

            await _areaService.DeleteArea(existingArea);
            response.Message = "Area is deleted";
            response.Success = true;
            return Ok(response);
        }
    }
}
