using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Authentication;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;

namespace SomoTaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMemberService _memberSerivce;
        private readonly IMemberServiceToken _memberServiceToken;
        private readonly ITokenHandler _tokenHandler;

        public LoginController(IMemberService memberService,IMemberServiceToken memberServiceToken,ITokenHandler tokenHandler)
        {
            _memberSerivce = memberService;
            _memberServiceToken = memberServiceToken;
            _tokenHandler = tokenHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            var member = await _memberSerivce.CheckLogin(loginModel.UserName, loginModel.Password);
            if(member == null)
            {
                return NotFound("Member not found");
            }
            (string accessToken, DateTime expiredDateAccess) = await _tokenHandler.CreateAccessToken(member);
            (string code, string refreshToken, DateTime expiredDateRefresh) = await _tokenHandler.CreateRefrehToken(member);

            await _memberServiceToken.SaveToken(new MemberToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiredDateAccessToken = expiredDateAccess,
                ExpiredDateRefreshToken = expiredDateRefresh,
                CreateDate = DateTime.Now,
                CodeRefreshToken = code,
                MemberId = member.Id,
                Name =member.Name,
                Status = 1
            });

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }


        [HttpPost("create")]
        public async Task<IActionResult> Check([FromBody] LoginModel loginModel)
        {
            var member = await _memberSerivce.GetByUser(loginModel.UserName, loginModel.Password);
            if (member == null)
            {
                return NotFound("Member not found");
            }

            return Ok(member);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            return Ok(await _tokenHandler.ValiDateRefrehToken(refreshToken));
        }
    }
}
