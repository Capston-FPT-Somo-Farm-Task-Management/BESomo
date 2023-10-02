using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFarmTaskService
    {
        Task Add(int memberId, TaskCreateUpdateModel farmTaskmodel, List<int> employeeIds, List<int> materialIds);
        Task Delete(FarmTask farmTask);
        Task<FarmTaskModel> Get(int id);
        Task<IEnumerable<FarmTaskModel>> GetList();
        Task<IEnumerable<FarmTaskModel>> GetListActive();
        Task<IEnumerable<FarmTaskModel>> GetTaskByDay(DateTime dateStr);
        Task<IEnumerable<FarmTaskModel>> GetTaskByMemberId(int id);
        Task<IEnumerable<FarmTaskModel>> GetListActiveByMemberId(int id);
        Task Update(int farmTaskId, int memberId, FarmTask farmTaskUpdate, List<int> employeeIds, List<int> materialIds);
        Task UpdateStatus(int id, int status);
    }
}
