using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveStockController : ControllerBase
    {
        private readonly ILiveStockService _liveStockService;

        public LiveStockController(ILiveStockService liveStockService)
        {
            _liveStockService = liveStockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _liveStockService.GetList());
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
                    return BadRequest("Livestock id must be greater than 0");
                }
                var area = await _liveStockService.Get(id);
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
        public async Task<IActionResult> GetLiveStockFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Livestock id must be greater than 0");
                }
                var area = await _liveStockService.GetLiveStockFarm(id);
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

        [HttpGet("Active")]
        public async Task<IActionResult> GetListActive()
        {
            try
            {
                var area = await _liveStockService.GetListActive();
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
        [HttpGet("ExternalId/Field({id})")]
        public async Task<IActionResult> GetExternalId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Field id must be greater than 0");
                }
                var area = await _liveStockService.GetExternalIds(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "External livestock is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LivestockCreateModel liveStock)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _liveStockService.Add(liveStock);
                    var responseData = new ApiResponseModel
                    {
                        Data = liveStock,
                        Message = "LiveStock is added",
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

                    response.Message = "Invalid LiveStock data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LiveStock liveStock)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {

                    if (id <= 0)
                    {
                        return BadRequest("Livestock id must be greater than 0");
                    }

                    var existing = await _liveStockService.Get(id);
                    if (existing == null)
                    {
                        response.Message = "Livestock not found";
                        return NotFound(response);
                    }
                    await _liveStockService.Update(liveStock);
                    var responseData = new ApiResponseModel
                    {
                        Data = liveStock,
                        Message = "LiveStock is updated",
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

                    response.Message = "Invalid livestock data: " + string.Join(" ", errorMessages);
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
                        return NotFound("Livestock id must be greater than 0 ");
                    }
                    await _liveStockService.UpdateStatus(id);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
