using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Member;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Authentication
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IMemberService _memberService;
        private readonly IMemberServiceToken _userServiceToken;
        private readonly IRoleService _roleService;

        public TokenHandler(IConfiguration configuration, IMemberService memberService, IMemberServiceToken userServiceToken, IRoleService roleService)
        {
            _configuration = configuration;
            _memberService = memberService;
            _userServiceToken = userServiceToken;
            _roleService = roleService;
        }

        public async Task<(string, DateTime)> CreateAccessToken(Member member)
        {
            DateTime expiredAccessToken = DateTime.Now.AddMinutes(15);
            string roleName = await _roleService.GetRoleNameById(member.RoleId);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss,_configuration["TokenBear:Issuer"] ,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(),ClaimValueTypes.Integer64,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["TokenBear:Audience"],ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp,expiredAccessToken.ToString("yyyy/MM/dd hh:mm:ss"),ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(ClaimTypes.Name, member.Name,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(ClaimTypes.Role, roleName,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim("Id", member.Id.ToString(),ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim("UserName", member.UserName,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenBear:SignatureKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["TokenBear:Issuer"],
                audience: _configuration["TokenBear:Issuer"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(15),
                credential
            );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenInfo);
            return await Task.FromResult((token, expiredAccessToken));
        }

        public async Task<(string, string, DateTime)> CreateRefrehToken(Member member)
        {
            DateTime expiredRefreshToken = DateTime.Now.AddHours(3);
            string code = Guid.NewGuid().ToString();

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, code,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss,_configuration["TokenBear:Issuer"] ,ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(),ClaimValueTypes.DateTime,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["TokenBear:Audience"],ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp,expiredRefreshToken.ToString("yyyy/MM/dd hh:mm:ss"),ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),
                new Claim(ClaimTypes.SerialNumber, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["TokenBear:Issuer"]),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenBear:SignatureKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["TokenBear:Issuer"],
                audience: _configuration["TokenBear:Issuer"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                credential
            );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenInfo);
            return await Task.FromResult((code, token, expiredRefreshToken));
        }

        public async Task ValidateToken(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();
            if (claims.Count == 0)
            {
                context.Fail("This token contains no information");
                return;
            }

            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity.FindFirst(JwtRegisteredClaimNames.Iss) == null)
            {
                context.Fail("This token is not issued by point entry");
                return;
            }

            if (identity.FindFirst("UserName") != null)
            {
                string userName = identity.FindFirst("UserName").Value;
                var user = await _memberService.FindByUserName(userName);

                if (user == null)
                {
                    context.Fail("This token is invalid for user");
                    return;
                }
            }

            if (identity.FindFirst(JwtRegisteredClaimNames.Exp) == null)
            {
                var dateExp = identity.FindFirst(JwtRegisteredClaimNames.Exp).Value;

                long ticks = long.Parse(dateExp);
                var date = DateTimeOffset.FromUnixTimeSeconds(ticks).DateTime;
                var minutes = date.Subtract(DateTime.Now).TotalMinutes;

                context.Fail("This token is expired");
                return;
            }
        }

        public async Task<JwtModel> ValiDateRefrehToken(string refreshToken)
        {
            JwtModel model = new();

            var claimPriciple = new JwtSecurityTokenHandler().ValidateToken(
                refreshToken,
                new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenBear:SignatureKey"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out _
             );

            if (claimPriciple == null) return model;
            string code = claimPriciple.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(code)) return model;

            MemberToken userToken = await _userServiceToken.CheckRefreshToken(code);

            if (userToken != null)
            {
                Member member = await _memberService.FindById(userToken.Id);
                (string newAccessToken, DateTime createdDate) = await CreateAccessToken(member);
                (string codeRefreshToken, string newRefreshToken, DateTime newDateCreated) = await CreateRefrehToken(member);

                return new JwtModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                };
            }

            return new();
        }
    }
}
