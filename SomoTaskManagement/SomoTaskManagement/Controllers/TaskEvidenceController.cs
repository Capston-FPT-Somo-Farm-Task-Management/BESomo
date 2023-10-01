using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskEvidenceController : ControllerBase
    {
        private readonly ITaskEvidenceService _taskEvidenceService;

        public TaskEvidenceController(ITaskEvidenceService taskEvidenceService)
        {
            _taskEvidenceService = taskEvidenceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _taskEvidenceService.ListTaskEvidence());
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
                    return NotFound("TaskEvidence is not found");
                }
                var area = await _taskEvidenceService.GetTaskEvidence(id);
                return Ok(new ApiResponseModel
                {
                    Data = area,
                    Message = "TaskEvidence is found",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] TaskEvidence taskEvidence)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _taskEvidenceService.AddTaskEvidencee(taskEvidence);
                    var responseData = new ApiResponseModel
                    {
                        Data = taskEvidence,
                        Message = "TaskEvidence is added",
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

                    response.Message = "Invalid TaskEvidence data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskEvidence taskEvidence)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {
                    var existingArea = await _taskEvidenceService.GetTaskEvidence(id);
                    if (existingArea == null)
                    {
                        response.Message = "TaskEvidence not found";
                        return NotFound(response);
                    }
                    await _taskEvidenceService.UpdateTaskEvidence(taskEvidence);
                    var responseData = new ApiResponseModel
                    {
                        Data = taskEvidence,
                        Message = "TaskEvidence is updated",
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

                    response.Message = "Invalid TaskEvidence data: " + string.Join(" ", errorMessages);
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
                var existingArea = await _taskEvidenceService.GetTaskEvidence(id);
                if (existingArea == null)
                {
                    response.Message = "TaskEvidence not found";
                    return NotFound(response);
                }

                await _taskEvidenceService.DeleteTaskEvidence(existingArea);
                response.Message = "TaskEvidence is deleted";
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
