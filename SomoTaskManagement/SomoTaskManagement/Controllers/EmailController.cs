using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Model.Reponse;
using SomoTaskManagement.Services.Imp;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService= emailService;
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult>SendMail(string email)
        {
            try
            {
                await _emailService.SendPasswordResetEmail(email);
                return Ok(new ApiResponseModel
                {
                    Data = null,
                    Message = "Gửi thành công",
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
    }
}
