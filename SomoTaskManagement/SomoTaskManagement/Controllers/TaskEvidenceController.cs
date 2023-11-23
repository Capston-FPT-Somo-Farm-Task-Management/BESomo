using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.EvidenceImage;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Impf;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Manager,Supervisor")]
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
            if (!User.IsInRole("Manager") &&!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                return Ok(await _taskEvidenceService.ListTaskEvidence());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Task({taskId})")]
        public async Task<IActionResult> GetEvidenceByTask(int taskId)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var area = await _taskEvidenceService.GetEvidenceByTask(taskId);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
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

        [HttpGet("countEvidence")]
        public async Task<IActionResult> CountEvidence()
        {
            var evidenceCount = await _taskEvidenceService.CountEvidenceOfTask();
            return Ok(new { EvidenceCount = evidenceCount });
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetStatusEvidenceDescriptions()
        {

            try
            {
                var task = _taskEvidenceService.GetStatusEvidenceDescriptions();

                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Danh sách trạng thái ",
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
        public async Task<IActionResult> CreateArea([FromBody] TaskEvidence taskEvidence)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var response = new ApiResponseModel();


                await _taskEvidenceService.AddTaskEvidencee(taskEvidence);
                var responseData = new ApiResponseModel
                {
                    Data = taskEvidence,
                    Message = "TaskEvidence is added",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("AddTaskEvidenceeWithImage")]
        public async Task<IActionResult> AddTaskEvidenceeWithImage([FromForm] EvidenceCreateUpdateModel evidenceCreateUpdateModel)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var response = new ApiResponseModel();


                await _taskEvidenceService.AddTaskEvidenceeWithImage(evidenceCreateUpdateModel);
                var responseData = new ApiResponseModel
                {
                    Data = evidenceCreateUpdateModel,
                    Message = "TaskEvidence is added",
                    Success = true,
                };
                return Ok(responseData);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm]TaskEvidenceUpdateModel taskEvidence)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {

                await _taskEvidenceService.UpdateTaskEvidence(id, taskEvidence.EvidenceCreateUpdateModel, taskEvidence.OldUrlImage);
                var responseData = new ApiResponseModel
                {
                    Data = taskEvidence,
                    Message = "TaskEvidence is updated",
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
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var response = new ApiResponseModel();

                await _taskEvidenceService.DeleteTaskEvidence(id);
                response.Message = "TaskEvidence is deleted";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("Disaggree/Task({id})")]
        public async Task<IActionResult> CreateDisagreeTask(int id, string description)
        {
            if (!User.IsInRole("Supervisor"))
            {
                return Unauthorized("Bạn không có quyền truy cập");
            }
            try
            {
                var response = new ApiResponseModel();

                await _taskEvidenceService.CreateDisagreeTask(id,description);
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
