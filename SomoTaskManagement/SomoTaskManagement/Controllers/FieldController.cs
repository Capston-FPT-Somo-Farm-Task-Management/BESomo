using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Field;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
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
                var field = await _fieldService.ListField();
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Tìm thấy",
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var field = await _fieldService.GetZoneField(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });
            }
        }
        [HttpGet("Plant/Farm({id})")]
        public async Task<IActionResult> GetPlantFieldByFarm(int id)
        {
            try
            {
                var field = await _fieldService.GetPlantFieldByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });
            }
        }
        [HttpGet("Livestock/Farm({id})")]
        public async Task<IActionResult> GetLivestockFieldByFarm(int id)
        {
            try
            {
                var field = await _fieldService.GetLivestockFieldByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });

            }
        }
        [HttpGet("Code({code})")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var field = await _fieldService.GetByCode(code);
                return Ok(new ApiResponseModel
                {
                    Data = field,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });

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
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
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
                await _fieldService.DeleteFieldByStatus(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });
            }

        }

        [HttpGet("Active/Zone({id})")]
        public async Task<IActionResult> GetByZone(int id)
        {
            try
            {
                var area = await _fieldService.GetByZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Error deleting record: " + e.Message,
                    Success = false
                });
            }
        }

        [HttpGet("Zone({id})")]
        public async Task<IActionResult> GetAllByZone(int id)
        {
            try
            {
                var area = await _fieldService.GetAllByZone(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });

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
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });
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
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });

            }
        }

        [HttpGet("Plant/Active")]
        public async Task<IActionResult> ListFieldPlantActive()
        {
            try
            {
                var area = await _fieldService.ListFieldPlantActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });
            }
        }

        [HttpGet("Livestock/Active")]
        public async Task<IActionResult> ListFieldLivestockActive()
        {
            try
            {
                var area = await _fieldService.ListFieldLivestockActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });

            }
        }


        [HttpGet("AreaZoneByField({id})")]
        public async Task<IActionResult> GetAreaZoneByField(int id)
        {
            try
            {
                var area = await _fieldService.GetAreaZoneByField(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thấy",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FieldCreateUpdateModel field)
        {
            try
            {

                await _fieldService.AddField(field);
                var responseData = new ApiResponseModel
                {
                    Data = field,
                    Message = "Thêm thành công",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FieldCreateUpdateModel field)
        {
            try
            {
                await _fieldService.UpdateField(id, field);
                var responseData = new ApiResponseModel
                {
                    Data = field,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi " + e.Message,
                    Success = false
                });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            try
            {
                await _fieldService.DeleteField(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponseModel
                {
                    Message = "Lỗi" + e.Message,
                    Success = false
                });
            }
        }

    }
}
