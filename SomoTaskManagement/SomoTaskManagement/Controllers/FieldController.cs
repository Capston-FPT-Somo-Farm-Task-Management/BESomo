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
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _fieldService.ListField());
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
                if (id <= 0)
                {
                    return NotFound("Field id nmust be greater than 0");
                }
                var field = await _fieldService.GetZoneField(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpGet("Plant/Farm({id})")]
        public async Task<IActionResult> GetPlantFieldByFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Farm id nmust be greater than 0");
                }
                var field = await _fieldService.GetPlantFieldByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpGet("Livestock/Farm({id})")]
        public async Task<IActionResult> GetLivestockFieldByFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Farm id nmust be greater than 0");
                }
                var field = await _fieldService.GetLivestockFieldByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var field = await _fieldService.ListFieldActive();
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Field is found",
                    Success = true,
                });
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
                        return NotFound("Field id must be greater than 0 ");
                    }
                    await _fieldService.DeleteFieldByStatus(id);
                    var responseData = new ApiResponseModel
                    {
                        Data = null,
                        Message = "Delete success",
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

                    response.Message = "Invalid Field data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("Active/Zone({id})")]
        public async Task<IActionResult> GetByZone(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id nmust be greater than 0");
                }
                var area = await _fieldService.GetByZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Zone({id})")]
        public async Task<IActionResult> GetAllByZone(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Zone id nmust be greater than 0");
                }
                var area = await _fieldService.GetAllByZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpGet("Plant")]
        public async Task<IActionResult> ListFieldPlant()
        {
            try
            {
                var area = await _fieldService.ListFieldPlant();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Livestock")]
        public async Task<IActionResult> ListFieldLivestock()
        {
            try
            {
                var area = await _fieldService.ListFieldLivestock();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("AreaZoneByField({id})")]
        public async Task<IActionResult> GetAreaZoneByField(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound("Field id must be greater than 0");
                }
                var area = await _fieldService.GetAreaZoneByField(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Field is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody]  Field field)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _fieldService.AddField(field);
                    var responseData = new ApiResponseModel
                    {
                        Data = field,
                        Message = "Field is added",
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

                    response.Message = "Invalid Field data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Field field)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _fieldService.GetZoneField(id);
                    if (existingArea == null)
                    {
                        response.Message = "Field not found";
                        return NotFound(response);
                    }
                    await _fieldService.UpdateField(field);
                    var responseData = new ApiResponseModel
                    {
                        Data = field,
                        Message = "Field is updated",
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

                    response.Message = "Invalid Field data: " + string.Join(" ", errorMessages);
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
        //        var existingArea = await _fieldService.GetZoneField(id);
        //        if (existingArea == null)
        //        {
        //            response.Message = "Field not found";
        //            return NotFound(response);
        //        }

        //        await _fieldService.DeleteField(existingArea);
        //        response.Message = "Field is deleted";
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
