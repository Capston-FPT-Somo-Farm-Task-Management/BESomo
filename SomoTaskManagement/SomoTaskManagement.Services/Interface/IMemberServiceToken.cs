using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IMemberServiceToken
    {
        Task<MemberToken> CheckRefreshToken(string code);
        Task SaveToken(MemberToken userToken);
    }
}
