using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ITaskTypeService
    {
        Task AddTaskType(TaskType taskType);
        Task DeleteTaskType(TaskType taskType);
        Task<TaskType> GetTaskType(int id);
        Task<IEnumerable<TaskType>> ListTaskType();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypeLivestock();
        Task<IEnumerable<TaskTypeModel>> ListTaskTypePlant();
        Task UpdateTaskType(TaskType taskType);
    }
}
