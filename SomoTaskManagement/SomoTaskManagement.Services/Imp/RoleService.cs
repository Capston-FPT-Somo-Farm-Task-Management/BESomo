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
    public class RoleService: IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string>GetRoleNameById(int id)
        {
            var role = await _unitOfWork.RepositoryRole.GetSingleByCondition(r => r.Id == id);
            var roleName = role.Name;   
            return roleName;
        }
    }
}
