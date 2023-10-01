using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class MemberTokenService: IMemberServiceToken
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SaveToken(MemberToken userToken)
        {
            await _unitOfWork.RepositoryUserToken.Add(userToken);
            await _unitOfWork.RepositoryUserToken.Commit();
        }

        public async Task<MemberToken> CheckRefreshToken(string code)
        {
            return await _unitOfWork.RepositoryUserToken.GetSingleByCondition(x => x.CodeRefreshToken == code);
        }
    }
}
