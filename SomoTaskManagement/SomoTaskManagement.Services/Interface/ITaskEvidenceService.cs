using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface ITaskEvidenceService
    {
        Task AddTaskEvidencee(TaskEvidence taskEvidence);
        Task DeleteTaskEvidence(TaskEvidence taskEvidence);
        Task<TaskEvidence> GetTaskEvidence(int id);
        Task<IEnumerable<TaskEvidence>> ListTaskEvidence();
        Task UpdateTaskEvidence(TaskEvidence taskEvidence);
    }
}
