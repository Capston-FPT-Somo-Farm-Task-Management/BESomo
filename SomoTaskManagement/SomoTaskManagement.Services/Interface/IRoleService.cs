using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IRoleService
    {
        Task<string> GetRoleNameById(int id);
    }
}
