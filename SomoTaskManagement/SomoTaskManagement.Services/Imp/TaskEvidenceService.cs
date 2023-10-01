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
    public class TaskEvidenceService: ITaskEvidenceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskEvidenceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<IEnumerable<TaskEvidence>> ListTaskEvidence()
        {
            return _unitOfWork.RepositoryTaskEvidence.GetData(null);
        }
        public Task<TaskEvidence> GetTaskEvidence(int id)
        {
            return _unitOfWork.RepositoryTaskEvidence.GetById(id);
        }
        public async Task AddTaskEvidencee(TaskEvidence taskEvidence)
        {
            taskEvidence.Status = 1;
            await _unitOfWork.RepositoryTaskEvidence.Add(taskEvidence);
        }
        public async Task UpdateTaskEvidence(TaskEvidence taskEvidence)
        {
            _unitOfWork.RepositoryTaskEvidence.Update(taskEvidence);
            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }
        public async Task DeleteTaskEvidence(TaskEvidence taskEvidence)
        {
            _unitOfWork.RepositoryTaskEvidence.Delete(a => a.Id == taskEvidence.Id);
            await _unitOfWork.RepositoryTaskEvidence.Commit();
        }
    }
}
