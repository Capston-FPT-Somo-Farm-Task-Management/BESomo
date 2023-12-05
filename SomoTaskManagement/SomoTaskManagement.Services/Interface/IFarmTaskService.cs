using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Domain.Model.TaskEvidence;
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
        Task UpdateStatusFormTodoToDraft(int id);
        Task<IEnumerable<FarmTaskModel>> GetTaskByTotalDay(DateTime date, int id);
        Task<IEnumerable<FarmTaskModel>> GetListActiveWithPagging(int pageIndex, int pageSize);
        Task<FarmTaskPageResult> GetTaskByStatusMemberDate(int id, int status, DateTime? date, int pageIndex, int pageSize, int? checkTaskParent, string? taskName);
        List<object> GetStatusDescriptions();
        Task<FarmTaskPageResult> GetTaskByStatusSupervisorDate(int id, int status, DateTime? date, int pageIndex, int pageSize, string? taskName);
        Task<int> CheckRoleMember(int id);
        Task CreateAsignTask(TaskCreateAsignModel taskModel, List<int>? materialIds, List<int>? employeeIds);
        Task CreateDisagreeTask(int id, string description);
        Task DisDisagreeTask(int id);
        Task<TaskByEmployeeDatesEffort> GetTaskByEmployeeDates(int employeeId, DateTime? startDay, DateTime? endDay, int pageIndex, int pageSize, int? status);
        Task<FarmTaskPageResult> GetAllTaskByMemberDate(int id, DateTime? date, int pageIndex, int pageSize, int? checkTaskParent, string? taskName);
        Task DeleteTask(int farmTaskId);
        Task<IEnumerable<FarmTaskModel>> GetTaskPrepareAndDoing(int id);
        Task<IEnumerable<TaskCountPerDayModel>> GetTotalTaskOfWeekByMember(int memberId);
        //Task<TotalTypeOfTaskInWeek> GetTotalTypeOfTaskInWeekByMember(int memberId);
        Task CreateTaskToDo(TaskToDoModel taskToDoModel, List<DateTime>? Dates, List<int>? materialIds);
        Task CreateTaskDraft(TaskDraftModel taskDraftModel, List<DateTime>? Dates, List<int>? materialIds);
        Task UpdateTask(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int> materialIds);
        Task UpdateTaskDraftAndToPrePare(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int>? materialIds);
        Task DeleteTaskTodoDraftAssign(int taskId);
        Task AddEmployeeToTaskAsign(int taskId, List<int>? employeeIds, int? overallEfforMinutes, int? overallEffortHour);
        Task ChangeStatusToDoing(int id);
        Task ChangeStatusToPendingAndCancel(int id, EvidencePendingAndCancel taskEvidence, int status, int? managerId);
        Task ChangeStatusFromDoneToDoing(int id, string description, int managerId);
        Task ChangeStatusToClose(int id);
        Task ChangeStatusToDone(int id);
        Task UpdateTaskDisagreeAndChangeToToDo(int taskId, TaskDraftModelUpdate taskModel, List<DateTime>? dates, List<int> materialIds);
        Task UpdateTaskAsign(int taskId, TaskUpdateAsignModel taskModel, List<int>? materialIds, List<int>? employeeIds);
        Task DeleteTaskAssign(int taskId);
        Task CreateTaskClone(int taskId);
        Task<IEnumerable<TaskCountPerDayModel>> GetTotalTaskOfFarm(int farmId);
        Task<TotalTaskOfMonth> GetTotalTaskOfFarmIncurrentMonth(int farmId, int month);
    }
}
