using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IFarmTaskService
    {
        Task Delete(FarmTask farmTask);
        Task<GetFarmTaskModel> Get(int id);
        Task<IEnumerable<FarmTaskModel>> GetList();
        Task<IEnumerable<FarmTaskModel>> GetListActive();
        Task<IEnumerable<FarmTaskModel>> GetTaskByDay(DateTime dateStr);
        Task<IEnumerable<FarmTaskModel>> GetTaskByMemberId(int id);
        Task<IEnumerable<FarmTaskModel>> GetListActiveByMemberId(int id);
        Task Update(int farmTaskId, int memberId, FarmTask farmTaskUpdate, List<int> employeeIds, List<int>? materialIds);
        Task UpdateStatus(int id, int status);
        Task<IEnumerable<FarmTaskModel>> GetTaskByTotalDay(DateTime date, int id);
        Task<IEnumerable<FarmTaskModel>> GetListActiveWithPagging(int pageIndex, int pageSize);
        Task<FarmTaskPageResult> GetTaskByStatusMemberDate(int id, int status, DateTime? date, int pageIndex, int pageSize, string? taskName);
        List<object> GetStatusDescriptions();
        Task<FarmTaskPageResult> GetTaskByStatusSupervisorDate(int id, int status, DateTime? date, int pageIndex, int pageSize, string? taskName);
        Task<int> CheckRoleMember(int id);
        Task ProcessTaskCreation(int memberId, TaskRequestModel taskModel);
        Task CreateDisagreeTask(int id, string description);
        Task DisDisagreeTask(int id);
        Task<TaskByEmployeeDatesEffort> GetTaskByEmployeeDates(int employeeId, DateTime? startDay, DateTime? endDay, int pageIndex, int pageSize, int? status);
    }
}
