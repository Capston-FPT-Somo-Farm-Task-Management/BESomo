using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;
using System.Globalization;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]

    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                return Ok(await _employeeService.ListEmployee());
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
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                var area = await _employeeService.GetEmployee(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Employee is found",
                    Success = true,
                });
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

        [HttpGet("EmployeeActive")]
        public async Task<IActionResult> GetEmployeeActive()
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var area = await _employeeService.ListEmployeeActive();
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Employee is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Active/TaskType({taskTypeId})/Farm({farmId})")]
        public async Task<IActionResult> ListByTaskTypeFarm(int taskTypeId, int farmId)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var employee = await _employeeService.ListByTaskTypeFarm(taskTypeId, farmId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = employee,
                    Message = "Employee is found",
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
        public async Task<IActionResult> ListEmployeeByFarm(int id)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                if (id <= 0)
                {
                    return NotFound("Farm id must be greater than 0");
                }
                var area = await _employeeService.ListEmployeeByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Employee is found",
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
        public async Task<IActionResult> ListEmployeeActiveByFarm(int id)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var area = await _employeeService.ListEmployeeActiveByFarm(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Employee is found",
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

        [HttpGet("EmployeeTaskType{id}")]
        public async Task<IActionResult> GetByTaskType(int id)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var employees = await _employeeService.GetByTaskType(id);

                return Ok(new ApiResponseModel
                {
                    Data = employees,
                    Message = "Employee is found",
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
        [HttpGet("Task({id})")]
        public async Task<IActionResult> ListEmployeeTask(int id)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var area = await _employeeService.ListTaskEmployee(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "Employee is found",
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
        public async Task<IActionResult> Create([FromForm] EmployeeCreateModel employee)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                await _employeeService.AddEmployee(employee);
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
                    Success = true,
                });
            }
        }

        [HttpGet("GetTotalEffortEmployee")]
        public async Task<IActionResult> GetTotalEffortEmployee(int id, int month, int year)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}

                var employeeEffort = await _employeeService.GetTotalEffortEmployee(id, month, year);
                var responseData = new ApiResponseModel
                {
                    Data = employeeEffort,
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
                    Success = true,
                });
            }
        }

        [HttpGet("Template")]
        public IActionResult GetExcelTemplate()
        {
            var numberOfDaysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("EmployeeImportTemplate");

                // Merge cells for the title
                worksheet.Cells["A1:L1"].Merge = true;
                worksheet.Cells[1, 1].Value = "BẢNG CHẤM CÔNG THÁNG 12 ";
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                using (var range = worksheet.Cells["A1:L1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 18;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                worksheet.Cells["A2:A3"].Merge = true;
                worksheet.Cells["B2:B3"].Merge = true;
                worksheet.Cells["C2:C3"].Merge = true;
                worksheet.Cells[2, 1].Value = "STT";
                worksheet.Cells[2, 2].Value = "Mã nhân viên";
                worksheet.Cells[2, 3].Value = "Họ tên";
                worksheet.Cells[2, 4 + numberOfDaysInMonth].Value = "Tổng ngày công";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Merge = true;
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Value = "NGÀY CÔNG";
                worksheet.Cells[2, 4, 2, 3 + numberOfDaysInMonth].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                using (var range = worksheet.Cells["A2:M2"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 12;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int currentColumn = 4;
                for (int day = 1; day <= numberOfDaysInMonth; day++)
                {
                    worksheet.Cells[3, currentColumn].Value = $"{day:00}";
                    currentColumn++;
                }

                using (var range = worksheet.Cells["A3:L3"])
                {
                    range.Style.Font.Size = 11;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //worksheet.Row(3).Height = 35;

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 30;
                for (int col = 4; col <= numberOfDaysInMonth + 2; col++)
                {
                    worksheet.Column(col).Width = 5;
                }

                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BangChamCong.xlsx");
            }
        }






        [HttpPost("ImortExcel")]
        public async Task<IActionResult> ImportEmployeesFromExcel([FromForm] EmployeeImportModel employee)
        {
            try
            {
                using (var stream = employee.ExcelFile.OpenReadStream())
                {
                    await _employeeService.ImportEmployeesFromExcel(stream);
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
                    Success = true,
                });
            }
        }

        [HttpGet("Export/{farmId}")]
        public async Task<IActionResult> ExportEmployeesToExcel(int farmId)
        {
            try
            {
                var excelData = await _employeeService.ExportEmployeesToExcel(farmId);

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

        [HttpGet("Effort/Farm({farmId})")]
        public async Task<IActionResult> ExportEmployeesEffortToExcel(int farmId, int month, int year)
        {
            try
            {
                var excelData = await _employeeService.ExportEmployeesEffortToExcel(farmId, month, year);

                var stream = new MemoryStream(excelData);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeEffort.xlsx");
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

        [HttpGet("Effort/Employee({employeeId})")]
        public async Task<IActionResult> ExportEmployeeEffort(int employeeId, int month, int year)
        {
            try
            {
                var excelData = await _employeeService.ExportEmployeeEffort(employeeId, month, year);

                var stream = new MemoryStream(excelData);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeEffort.xlsx");
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
        public async Task<IActionResult> Update(int id, [FromForm] EmployeeCreateModel employeeUpdateRequest)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                await _employeeService.UpdateEmployee(id, employeeUpdateRequest);
                var responseData = new ApiResponseModel
                {
                    Data = employeeUpdateRequest,
                    Message = "Employee is updated",
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

        [HttpGet("GetEffortEmployeeInTask")]
        public async Task<IActionResult> GetEffortEmployeeInTask(int id, int month, int year)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
              var task=  await _employeeService.GetEffortEmployeeInTask(id, month,year);
                var responseData = new ApiResponseModel
                {
                    Data = task,
                    Message = "Employee is updated",
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
        //        //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
        //        //{
        //        //    return Unauthorized("You do not have access to this method.");
        //        //}
        //        var response = new ApiResponseModel();
        //        var existingArea = await _employeeService.GetEmployee(id);
        //        if (existingArea == null)
        //        {
        //            response.Message = "Employee not found";
        //            return NotFound(response);
        //        }

        //        await _employeeService.DeleteEmployee(existingArea);
        //        response.Message = "Employee is deleted";
        //        response.Success = true;
        //        return Ok(response);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
            //{
            //    return Unauthorized("You do not have access to this method.");
            //}
            try
            {
                await _employeeService.UpdateStatus(id);
                var responseData = new ApiResponseModel
                {
                    Data = null,
                    Message = "Status employee is updated",
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
