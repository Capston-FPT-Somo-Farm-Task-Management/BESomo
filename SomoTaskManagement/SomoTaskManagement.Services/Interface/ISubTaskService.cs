using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.SubTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ISubTaskService
    {
        Task CreateSubTasks(SubTaskCreateModel subTask);
        Task DeleteSubTasks(int subtaskId);
        Task<GetEffortByTaskModel> GetEffortByTask(int taskId);
        Task<IEnumerable<EmployeeListModel>> GetEmployeesNoSubtask(int taskId);
        Task<TotalEffortModel> GetTotalEffortEmployee(int id, DateTime? startDay, DateTime? endDay);
        Task<IEnumerable<SubTaskModel>> NonSubtaskByTask(int taskId);
        Task<IEnumerable<SubTaskModel>> SubtaskByTask(int taskId);
        Task UpdateEffortOfSubtask(int subtaskId, EmployeeEffortUpdate employeeEffortTime);
        Task UpdateEffortTime(int taskId, List<EmployeeEffortUpdate> employeeEffortTimes);
        Task UpdateEffortTimeAndStatusTask(int taskId, List<EmployeeEffortUpdateAndChangeStatus> employeeEffortTimes);
        Task UpdateSubTasks(int subtaskId, SubTaskUpdateModel subTask);
    }
}
