using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Repo
{
    public class TaskRepository
    {
        private readonly string connectionString;
        private readonly SomoTaskManagemnetContext dbContext;

        public TaskRepository(string connectionString, SomoTaskManagemnetContext _dbContext)
        {
            this.connectionString = connectionString;
            this.dbContext = _dbContext;
        }

        public List<FarmTask> GetTasks()
        {
            var taskList = dbContext.FarmTask.ToList();
            foreach (var task in taskList)
            {
                dbContext.Entry(task).Reload();
            }
            return taskList;
        }
    }
}
