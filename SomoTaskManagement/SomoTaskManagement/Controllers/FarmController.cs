﻿using DemoRedis.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Domain.Model.Farm;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin,Supervisor")]
    public class FarmController : ControllerBase
    {
        private readonly IFarmService _farmService;

        public FarmController(IFarmService farmService)
        {
            _farmService = farmService;
        }
        [HttpGet]
        //[Cache]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _farmService.ListFarm());
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
                var area = await _farmService.GetFarmById(id);
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
        public async Task<IActionResult> CreateArea([FromForm] FarmCreateUpdateModel farm)
        {
            try
            {
                var response = new ApiResponseModel();
                if (ModelState.IsValid)
                {

                    await _farmService.CreateFarm(farm);
                    var responseData = new ApiResponseModel
                    {
                        Data = farm,
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

                    response.Message = "Invalid Farm data: " + string.Join(" ", errorMessages);
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] FarmCreateUpdateModel farm)
        {
            try
            {
                await _farmService.UpdateFarm(id,farm);
                var responseData = new ApiResponseModel
                {
                    Data = farm,
                    Message = "Farm is updated",
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
                var existingArea = await _farmService.GetFarmById(id);
                if (existingArea == null)
                {
                    response.Message = "Không tìm thấy trang trại";
                    return NotFound(response);
                }

                await _farmService.DeleteFarm(existingArea);
                response.Message = "Xóa thành công";
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
