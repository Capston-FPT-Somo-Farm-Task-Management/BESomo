﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskTypeController : ControllerBase
    {
        private readonly ITaskTypeService _taskTypeService;

        public TaskTypeController(ITaskTypeService taskTypeService)
        {
            _taskTypeService = taskTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _taskTypeService.ListTaskType());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("ListTaskTypePlant")]
        public async Task<IActionResult> ListTaskTypePlant()
        {
            try
            {
                var members = await _taskTypeService.ListTaskTypePlant();
                return Ok(new ApiResponseModel
                {
                    Data = members,
                    Message = "List task type of plant ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpGet("Active")]
        public async Task<IActionResult> ListTaskTypeActive()
        {
            try
            {
                var members = await _taskTypeService.ListTaskTypeActive();
                return Ok(new ApiResponseModel
                {
                    Data = members,
                    Message = "List task type of plant ",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Export")]
        public async Task<IActionResult> ExportTaskTypeToExcel()
        {
            try
            {
                var excelData = await _taskTypeService.ExportTaskTypeToExcel();

                var stream = new MemoryStream(excelData);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TaskType.xlsx");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        [HttpGet("Template")]
        public IActionResult GetExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaskTypeTemplate");

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã";
                worksheet.Cells[1, 3].Value = "Tên";
                worksheet.Cells[1, 4].Value = "Loại công việc";
                worksheet.Cells[1, 5].Value = "Mô tả";

                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TaskTypeTemplate.xlsx");
            }
        }

        [HttpPost("ImortExcel")]
        public async Task<IActionResult> ImportEmployeesFromExcel([FromForm] EmployeeImportModel employee)
        {
            try
            {
                using (var stream = employee.ExcelFile.OpenReadStream())
                {
                    await _taskTypeService.ImportTaskTypeFromExcel(stream);
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

        [HttpGet("ListTaskTypeLivestock")]
        public async Task<IActionResult> ListTaskTypeLivestock()
        {
            try
            {
                var members = await _taskTypeService.ListTaskTypeLivestock();
                return Ok(new ApiResponseModel
                {
                    Data = members,
                    Message = "List task type of livestock ",
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
                    return NotFound("TaskType is not found");
                }
                var area = await _taskTypeService.GetTaskType(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "TaskType is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] TaskType taskType)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _taskTypeService.AddTaskType(taskType);
                    var responseData = new ApiResponseModel
                    {
                        Data = taskType,
                        Message = "TaskType is added",
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

                    response.Message = "Invalid TaskType data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskType taskType)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _taskTypeService.GetTaskType(id);
                    if (existingArea == null)
                    {
                        response.Message = "TaskType not found";
                        return NotFound(response);
                    }
                    await _taskTypeService.UpdateTaskType(taskType);
                    var responseData = new ApiResponseModel
                    {
                        Data = taskType,
                        Message = "TaskType is updated",
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

                    response.Message = "TaskType Zone data: " + string.Join(" ", errorMessages);
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
                var existingArea = await _taskTypeService.GetTaskType(id);
                if (existingArea == null)
                {
                    response.Message = "TaskType not found";
                    return NotFound(response);
                }

                await _taskTypeService.DeleteTaskType(existingArea);
                response.Message = "TaskType is deleted";
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
