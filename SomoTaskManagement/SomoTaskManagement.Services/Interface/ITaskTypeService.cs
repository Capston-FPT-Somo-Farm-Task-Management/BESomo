using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.TaskType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ITaskTypeService
    {
        Task AddTaskType(TaskTypeCreateUpdateModel taskType);
        Task DeleteTaskType(int taskTypeId);
        Task<byte[]> ExportTaskTypeToExcel();
        Task<TaskType> GetTaskType(int id);
        Task ImportTaskTypeFromExcel(Stream excelFileStream);
        Task<IEnumerable<TaskTypeModel>> ListTaskType();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypeActive();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypeLivestock();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypeOther();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypePlant();
        Task UpdateStatus(int id);
        Task UpdateTaskType(int taskTypeId, TaskTypeCreateUpdateModel taskType);
    }
}
