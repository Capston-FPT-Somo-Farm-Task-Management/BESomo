using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtherController : ControllerBase
    {
        private readonly IOtherService _otherService;

        public OtherController(IOtherService otherService)
        {
            _otherService = otherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _otherService.ListOther());
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
                var area = await _otherService.GetOther(id);
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
        public async Task<IActionResult> CreateArea([FromBody] Other other)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _otherService.AddOther(other);
                    var responseData = new ApiResponseModel
                    {
                        Data = other,
                        Message = "Other is added",
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

                    response.Message = "Invalid Other data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Other other)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _otherService.GetOther(id);
                    if (existingArea == null)
                    {
                        response.Message = "Other not found";
                        return NotFound(response);
                    }
                    await _otherService.UpdateOther(other);
                    var responseData = new ApiResponseModel
                    {
                        Data = other,
                        Message = "Other is updated",
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

                    response.Message = "Invalid Other data: " + string.Join(" ", errorMessages);
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
                var existingArea = await _otherService.GetOther(id);
                if (existingArea == null)
                {
                    response.Message = "Other not found";
                    return NotFound(response);
                }

                await _otherService.DeleteOther(existingArea);
                response.Message = "Other is deleted";
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
