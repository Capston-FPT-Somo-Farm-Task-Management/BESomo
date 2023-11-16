using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Livestock;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
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
                var area = await _liveStockService.GetList();
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
                var area = await _liveStockService.Get(id);
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
        public async Task<IActionResult> GetLiveStockActiveFarm(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Livestock id must be greater than 0");
                }
                var area = await _liveStockService.GetLiveStockActiveFarm(id);
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
                var area = await _liveStockService.GetListActive();
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
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
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
                        Message = "Thêm thành công",
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
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LivestockCreateModel liveStock)
        {
            try
            {
                await _liveStockService.Update(id, liveStock);
                var responseData = new ApiResponseModel
                {
                    Data = liveStock,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Data = liveStock,
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
                await _liveStockService.UpdateStatus(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Đã xóa",
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
    }
}
