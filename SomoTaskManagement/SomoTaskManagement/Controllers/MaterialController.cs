using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var material = await _materialService.ListMaterial();
                return Ok(new ApiResponseModel
                {
                    Data = material,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("Active/Farm({farmid})")]
        public async Task<IActionResult> ListMaterialActive(int farmid)
        {
            try
            {
                var material = await _materialService.ListMaterialActive(farmid);
                return Ok(new ApiResponseModel
                {
                    Data = material,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Farm({farmid})")]
        public async Task<IActionResult> ListMaterialByFarm(int farmid)
        {
            try
            {
                var material = await _materialService.ListMaterialByFarm(farmid);
                return Ok(new ApiResponseModel
                {
                    Data = material,
                    Message = "Material is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Farm({farmid})/Export")]
        public async Task<IActionResult> ExportMaterialToExcel(int farmid)
        {
            try
            {
                var excelData = await _materialService.ExportMaterialToExcel(farmid);

                var stream = new MemoryStream(excelData);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeExport.xlsx");
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

        [HttpPost("{farmId}/ImportExcel")]
        public async Task<IActionResult> ImportMaterialFromExcel([FromForm] EmployeeImportModel employee, int farmId)
        {
            try
            {
                if (!employee.ExcelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ApiResponseModel
                    {
                        Data = null,
                        Message = "Invalid file format. Please upload an Excel file.",
                        Success = false,
                    });
                }

                using (var stream = employee.ExcelFile.OpenReadStream())
                {
                    await _materialService.ImportMaterialFromExcel(stream, farmId);
                }

                var responseData = new ApiResponseModel
                {
                    Data = employee,
                    Message = "Employee is added",
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
                    Success = false,
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var area = await _materialService.GetMaterial(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Tìm thành công",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromForm] MaterialCreateUpdateModel material)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _materialService.AddMaterial(material);
                    var responseData = new ApiResponseModel
                    {
                        Data = material,
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

                    response.Message = "Invalid Material data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] MaterialCreateUpdateModel material)
        {
            try
            {
                await _materialService.UpdateMaterial(id, material);
                var responseData = new ApiResponseModel
                {
                    Data = material,
                    Message = "Cập nhật thành công",
                    Success = true,
                };
                return Ok(responseData);
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
                await _materialService.DeleteByStatus(id);
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
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = new ApiResponseModel();
                var existingArea = await _materialService.GetMaterial(id);
                if (existingArea == null)
                {
                    response.Message = "Material not found";
                    return NotFound(response);
                }

                await _materialService.DeleteMaterial(existingArea);
                response.Message = "Material is deleted";
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
