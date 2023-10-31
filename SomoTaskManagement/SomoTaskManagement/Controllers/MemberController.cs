﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Member;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
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
                return BadRequest(new ApiResponseModel
                {
                    Data = null,
                    Message = e.Message,
                    Success = true,
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                var member = await _memberService.GetById(id);
                return Ok(new ApiResponseModel
                {
                    Data = member,
                    Message = "Member is founded",
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

        [HttpGet("Supervisor/Farm({id})")]
        public async Task<IActionResult> GetSuperviosr(int id)
        {
            try
            {
                var supervisor = await _memberService.ListSupervisor(id);
                return Ok(new ApiResponseModel
                {
                    Data = supervisor,
                    Message = "Supervisor is founded",
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
        [HttpGet("Active/Supervisor/Farm({id})")]
        public async Task<IActionResult> ListSupervisorActive(int id)
        {
            try
            {
                var supervisor = await _memberService.ListSupervisorActive(id);
                return Ok(new ApiResponseModel
                {
                    Data = supervisor,
                    Message = "Supervisor is founded",
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
        public async Task<IActionResult> Create([FromBody] MemberCreateUpdateModel member)
        {
            try
            {

                await _memberService.CreateMember(member);
                var responseData = new ApiResponseModel
                {
                    Data = member,
                    Message = "Người dùng tạo thành công",
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
        public async Task<IActionResult> Update(int id, [FromBody] MemberCreateUpdateModel liveStock)
        {
            try
            {
                await _memberService.UdateMember(id, liveStock);
                var responseData = new ApiResponseModel
                {
                    Data = liveStock,
                    Message = "Người dùng đã cập nhật thành công",
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
    }
}
