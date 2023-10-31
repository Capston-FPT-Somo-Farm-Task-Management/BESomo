using SomoTaskManagement.Domain.Entities;

namespace SomoTaskManagement.Services.Repo
{
    public interface ITaskRepository
    {
        List<FarmTask> GetTasks();
        Task<List<Notification>> List();
    }
}