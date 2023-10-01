using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var members = await _memberService.List();
                return Ok(new ApiResponseModel
                {
                    Data = members,
                    Message = "List member success",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Member id must be greater than 0");
                }

                var task = await _memberService.GetById(id);
                if (task == null)
                {
                    return BadRequest("Not found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = task,
                    Message = "Member is founded",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("Supervisor/Farm({id})")]
        public async Task<IActionResult> GetSuperviosr(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Farm id must be greater than 0");
                }

                var supervisor = await _memberService.ListSupervisor(id);
                if (supervisor == null)
                {
                    return BadRequest("Supervisor found");
                }
                return Ok(new ApiResponseModel
                {
                    Data = supervisor,
                    Message = "Supervisor is founded",
                    Success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Member member)
        {
            try
            {
                var response = new ApiResponseModel();

                if (ModelState.IsValid)
                {
                    await _memberService.CreateMember(member);
                    var responseData = new ApiResponseModel
                    {
                        Data = member,
                        Message = "Member is added",
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

                    response.Message = "Invalid member data: " + string.Join(" ", errorMessages);
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
