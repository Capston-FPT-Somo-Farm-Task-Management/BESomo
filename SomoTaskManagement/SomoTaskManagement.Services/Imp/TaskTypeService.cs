using AutoMapper;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.TaskType;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class TaskTypeService: ITaskTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskTypeModel>> ListTaskType()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(null);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypePlant()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression:e=>e.Status ==0);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeActive()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeLivestock()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.Status == 1);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }

        public Task<TaskType> GetTaskType(int id)
        {
            return _unitOfWork.RepositoryTaskTaskType.GetById(id);
        }
        public async Task AddTaskType(TaskType taskType)
        {
            taskType.Status = 1;
            await _unitOfWork.RepositoryTaskTaskType.Add(taskType);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task UpdateTaskType(TaskType taskType)
        {
            _unitOfWork.RepositoryTaskTaskType.Update(taskType);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task DeleteTaskType(TaskType taskType)
        {
            _unitOfWork.RepositoryTaskTaskType.Delete(a => a.Id == taskType.Id);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
    }
}
