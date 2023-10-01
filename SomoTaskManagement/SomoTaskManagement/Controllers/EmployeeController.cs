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
                if (id == 0)
                {
                    return NotFound("Employee is not found");
                }
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
                return BadRequest(e.Message);

            }
        }

        [HttpGet("TaskType({taskTypeId})/Farm({farmId})")]
        public async Task<IActionResult> ListByTaskTypeFarm(int taskTypeId, int farmId)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                
                var employee = await _employeeService.ListByTaskTypeFarm(taskTypeId, farmId);
                if(employee == null)
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
                return BadRequest(e.Message);

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
                return BadRequest(e.Message);

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
                if (id <= 0)
                {
                    return NotFound("Employee id must be greater than 0");
                }
                var area = await _employeeService.GetByTaskType(id);
                if(area == null)
                {
                    return NotFound("Employee is not foundesd");
                }
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequestModel employee)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _employeeService.AddEmployee(employee.TaskTypeId,employee.Employee);
                    var responseData = new ApiResponseModel
                    {
                        Data = employee,
                        Message = "Employee is added",
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

                    response.Message = "Invalid Employee data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            try
            {
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _employeeService.GetEmployee(id);
                    if (existingArea == null)
                    {
                        response.Message = "Employee not found";
                        return NotFound(response);
                    }
                    await _employeeService.UpdateEmployee(employee);
                    var responseData = new ApiResponseModel
                    {
                        Data = employee,
                        Message = "Employee is updated",
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

                    response.Message = "Invalid Employee data: " + string.Join(" ", errorMessages);
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
                //if (!User.IsInRole("Manager") && !User.IsInRole("Admin"))
                //{
                //    return Unauthorized("You do not have access to this method.");
                //}
                var response = new ApiResponseModel();
                var existingArea = await _employeeService.GetEmployee(id);
                if (existingArea == null)
                {
                    response.Message = "Employee not found";
                    return NotFound(response);
                }

                await _employeeService.DeleteEmployee(existingArea);
                response.Message = "Employee is deleted";
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
