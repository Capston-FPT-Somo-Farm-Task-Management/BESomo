using Microsoft.AspNetCore.Authentication.JwtBearer;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Member;

namespace SomoTaskManagement.Authentication
{
    public interface ITokenHandler
    {
        Task<(string, DateTime)> CreateAccessToken(Member member);
        Task<(string, string, DateTime)> CreateRefrehToken(Member member);
        Task<JwtModel> ValiDateRefrehToken(string refreshToken);
        Task ValidateToken(TokenValidatedContext context);
    }
}