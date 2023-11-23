using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.EvidenceImage;
using SomoTaskManagement.Domain.Model.TaskEvidence;
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
        Task AddTaskEvidenceeWithImage(EvidenceCreateUpdateModel evidenceCreateUpdateModel);
        Task<int> CountEvidenceOfTask();
        Task CreateDisagreeTask(int id, string description);
        Task DeleteTaskEvidence(int id);
        Task<IEnumerable<TaskEvidenceModel>> GetEvidenceByTask(int taskId);
        List<object> GetStatusEvidenceDescriptions();
        Task<TaskEvidenceModel> GetTaskEvidence(int id);
        Task<IEnumerable<TaskEvidence>> ListTaskEvidence();
        Task UpdateTaskEvidence(int id, EvidenceCreateUpdateModel taskEvidence, List<string> oldUrlImages);
    }
}
