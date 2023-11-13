using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Imp;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm]EmployeeCreateModel employeeUpdateRequest)
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
